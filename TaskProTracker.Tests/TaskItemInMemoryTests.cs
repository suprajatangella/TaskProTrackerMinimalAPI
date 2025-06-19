using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Endpoints;
using TaskProTracker.MinimalAPI.Models;
using TaskProTracker.Tests.Helpers;

namespace TaskProTracker.Tests
{
    public class TaskItemInMemoryTests
    {
        [Fact]
        public async Task GetAllReturnsTasksFromDatabase()
        {
            // Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title", ProjectId = 1, IsCompleted = false });
            context.Tasks.Add(new TaskItem { Id = 2, Title = "Test title", ProjectId = 1, IsCompleted = false });

            await context.SaveChangesAsync();

            // Act
            var result = await TaskEndpoints.GetAllTasks(context);

            //Assert
            Assert.IsType<Ok<TaskItem[]>>(result);
            var okResult = result as Ok<TaskItem[]>;

            Assert.NotNull(okResult?.Value);
            Assert.NotEmpty(okResult.Value);
            Assert.Collection(okResult.Value, task1 =>
            {
                Assert.Equal(1, task1.Id);
                Assert.Equal("Test title 1", task1.Title);
                Assert.False(task1.IsCompleted);
            }, task2 =>
            {
                Assert.Equal(2, task2.Id);
                Assert.Equal("Test title 2", task2.Title);
                Assert.True(task2.IsCompleted);
            });
        }
        [Fact]
        public async Task GetTaskItemReturnsTaskFromDatabase()
        {
            // Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1,  Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role="Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description="Creating Unit tests", UserId = 1 });
            context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title", ProjectId = 1, IsCompleted = false });

            await context.SaveChangesAsync();

            // Act
            var result = await TaskEndpoints.GetTask(1, context);

            //Assert
            Assert.IsType<Ok<TaskItem>>(result);

            var okResult = result as Ok<TaskItem>;

            Assert.NotNull(okResult?.Value);
        }
        [Fact]
        public async Task CreateTaskCreatesTaskInDatabase()
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

            Assert.NotNull(result);
            //Assert.NotNull(result.Location);

            Assert.NotEmpty(context.Tasks);
            Assert.Collection(context.Tasks, task =>
            {
                Assert.Equal("Test title", task.Title);
                Assert.Equal(1, task.ProjectId);
                Assert.False(task.IsCompleted);
            });
        }
    }
}
