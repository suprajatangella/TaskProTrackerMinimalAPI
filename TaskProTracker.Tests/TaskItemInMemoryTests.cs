﻿using Microsoft.AspNetCore.Http.HttpResults;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Endpoints;
using TaskProTracker.MinimalAPI.Models;
using TaskProTracker.Tests.Helpers;

namespace TaskProTracker.Tests
{
    [Collection("Logger Collection")]
    public class TaskItemInMemoryTests
    {
        private readonly Serilog.ILogger _log;
        public TaskItemInMemoryTests(LoggerFixture log)
        {
            _log = log.Logger;
        }
        [Fact]
        public async Task GetTaskReturnsNotFoundIfNotExists()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                // Act
                var result = await TaskEndpoints.GetTask(1, context);

                //Assert
                Assert.IsType<Results<Ok<TaskItem>, NotFound>>(result);

                var notFoundResult = (NotFound)result.Result;

                Assert.NotNull(notFoundResult);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetTaskReturnsNotFoundIfNotExists test method");
                throw;
            }

        }
        [Fact]
        public async Task GetAllReturnsTasksFromDatabase()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                context.Tasks.Add(new TaskItem { Id = 2, Title = "Test title 2", ProjectId = 1, IsCompleted = false });

                await context.SaveChangesAsync();

                // Act
                var result = await TaskEndpoints.GetAllTasks(context);

                //Assert
                Assert.IsType<Results<Ok<List<TaskItem>>, NotFound>>(result);
                if (result.Result is Ok<List<TaskItem>> okResult)
                {
                    var tasks = okResult.Value;
                    Assert.NotEmpty(tasks!);
                    Assert.Collection(tasks!,
                        task1 =>
                        {
                            Assert.Equal(1, task1.Id);
                            Assert.Equal("Test title 1", task1.Title);
                            Assert.False(task1.IsCompleted);
                        },
                        task2 =>
                        {
                            Assert.Equal(2, task2.Id);
                            Assert.Equal("Test title 2", task2.Title);
                            Assert.False(task2.IsCompleted);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetAllReturnsTasksFromDatabase test method");
                throw;
            }
        }
        [Fact]
        public async Task GetTaskItemReturnsTaskFromDatabase()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title", ProjectId = 1, IsCompleted = false });

                await context.SaveChangesAsync();

                // Act
                var result = await TaskEndpoints.GetTask(1, context);

                //Assert
                Assert.IsType<Results<Ok<TaskItem>, NotFound>>(result);

                var okResult = (Ok<TaskItem>)result.Result;

                Assert.NotNull(okResult.Value);
                Assert.Equal(1, okResult.Value.Id);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetTaskItemReturnsTaskFromDatabase test method");
                throw;
            }
        }
        [Fact]
        public async Task CreateTaskCreatesTaskInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                var newTask = new TaskItemDTO { Title = "Test title", ProjectId = 1, IsCompleted = false };

                //Act
                var result = await TaskEndpoints.CreateTask(newTask, context);

                //Assert
                Assert.IsType<Created<TaskItem>>(result);

                Assert.NotNull(result.Value);
                Assert.NotNull(result.Location);

                Assert.NotEmpty(context.Tasks);
                Assert.Collection(context.Tasks, task =>
                {
                    Assert.Equal("Test title", task.Title);
                    Assert.Equal(1, task.ProjectId);
                    Assert.False(task.IsCompleted);
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in CreateTaskCreatesTaskInDatabase test method");
                throw;
            }
        }

        [Fact]
        public async Task UpdateTaskUpdatesTaskInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                var newTask = new TaskItem { Title = "Test title", ProjectId = 1, IsCompleted = false };
                context.Tasks.Add(newTask);

                await context.SaveChangesAsync();

                var updatedTaskDto = new TaskItemDTO
                {
                    Id = 1,
                    Title = "Updated test title",
                    IsCompleted = true,
                    ProjectId = 1
                };

                //Act
                var result = await TaskEndpoints.UpdateTask(updatedTaskDto.Id, updatedTaskDto, context);

                //Assert
                Assert.IsType<Results<Created<TaskItem>, NotFound>>(result);

                var created = (Created<TaskItem>)result.Result;
                Assert.NotNull(created.Value);
                Assert.NotNull(created.Location);
                var taskInDb = await context.Tasks.FindAsync(1);

                Assert.NotNull(taskInDb);
                Assert.Equal(1, taskInDb.Id);
                Assert.Equal("Updated test title", taskInDb!.Title);
                Assert.Equal(1, taskInDb!.ProjectId);
                Assert.True(taskInDb.IsCompleted);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in UpdateTaskUpdatesTaskInDatabase test method");
                throw;
            }
        }

        [Fact]
        public async Task DeleteTaskDeletesTaskInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                var existingTask = new TaskItem
                {
                    Id = 1,
                    Title = "Exiting test title",
                    ProjectId = 1,
                    IsCompleted = false
                };

                context.Tasks.Add(existingTask);

                await context.SaveChangesAsync();

                //Act
                var result = await TaskEndpoints.DeleteTask(existingTask.Id, context);

                //Assert
                Assert.IsType<Results<NoContent, NotFound>>(result);

                var noContentResult = (NoContent)result.Result;

                Assert.NotNull(noContentResult);
                Assert.Empty(context.Tasks);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in DeleteTaskDeletesTaskInDatabase test method");
                throw;
            }
        }
    }
}
