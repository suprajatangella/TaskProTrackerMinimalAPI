using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    public class ProjectInMemoryTests
    {
        [Fact]
        public async Task GetProjectReturnsNotFoundIfNotExists()
        {
            // Arrange
            await using var context = new MockDb().CreateDbContext();

            // Act
            var result = await ProjectEndpoints.GetProject(1, context);

            //Assert
            Assert.IsType<Results<Ok<Project>, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;

            Assert.NotNull(notFoundResult);
        }
        [Fact]
        public async Task GetAllReturnsProjectsFromDatabase()
        {
            // Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project 1", Description = "Creating Unit tests", UserId = 1 });
            context.Projects.Add(new Project { Id = 2, Title = "Test Project 2", Description = "Creating integration tests", UserId = 1 });

            await context.SaveChangesAsync();

            // Act
            var result = await ProjectEndpoints.GetAllProjects(context);

            //Assert
            Assert.IsType<Results<Ok<List<Project>>, NotFound>>(result);

            var okResult = (Ok<List<Project>>)(result.Result); // Cast the Results
            var projects = Assert.IsAssignableFrom<IEnumerable<Project>>(okResult.Value); // Get the data

            Assert.NotEmpty(projects); // Now this works, because 'todos' is IEnumerable
            Assert.Collection(projects,
                project1 =>
                {
                    Assert.Equal(1, project1.Id);
                    Assert.Equal("Test Project 1", project1.Title);
                    Assert.Equal(1, project1.UserId);
                },
                project2 =>
                {
                    Assert.Equal(2, project2.Id);
                    Assert.Equal("Test Project 2", project2.Title);
                    Assert.Equal(1, project2.UserId);
                }
            );

        }
        [Fact]
        public async Task GetProjectReturnsProjectFromDatabase()
        {
            // Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1,  Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role="Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description="Creating Unit tests", UserId = 1 });
            //context.Tasks.Add(new Project { Id = 1, Title = "Test title", ProjectId = 1, IsCompleted = false });

            await context.SaveChangesAsync();

            // Act
            var result = await ProjectEndpoints.GetProject(1, context);

            //Assert
            Assert.IsType<Results<Ok<Project>, NotFound>>(result);

            var okResult = (Ok<Project>)result.Result;

            Assert.NotNull(okResult.Value);
            Assert.Equal(1, okResult.Value.Id);
        }
        [Fact]
        public async Task CreateProjectCreatesProjectInDatabase()
        {
            //Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
            var newProj = new Project { Title = "Test Project", Description = "Creating Unit tests", UserId = 1 };
            //var newTask = new ProjectDTO { Title = "Test title", ProjectId = 1, IsCompleted = false };

            //Act
            var result = await ProjectEndpoints.CreateProject(newProj, context);

            //Assert
            Assert.IsType<Created<Project>>(result);

            Assert.NotNull(result);
            Assert.NotNull(result.Location);

            Assert.NotEmpty(context.Projects);
            Assert.Collection(context.Projects, project =>
            {
                Assert.Equal("Test Project", project.Title);
                Assert.Equal(1, project.Id);
                Assert.Equal(1, project.UserId);
            });
        }

        [Fact]
        public async Task UpdateProjectUpdatesProjectInDatabase()
        {
            //Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
            
            await context.SaveChangesAsync();

            var updatedProj = new Project
            {
                Id = 1,
                Title = "Updated Test Project",
                UserId = 1
            };

            //Act
            var result = await ProjectEndpoints.UpdateProject(updatedProj.Id, updatedProj, context);

            //Assert
            Assert.IsType<Results<Created<Project>, NotFound>>(result);

            var createdResult = (Created<Project>)result.Result;

            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Location);

            var projInDb = await context.Projects.FindAsync(1);

            Assert.NotNull(projInDb);
            Assert.Equal("Updated Test Project", projInDb!.Title);
            Assert.Equal(1, projInDb.UserId);
        }

        [Fact]
        public async Task DeleteProjectDeletesProjectInDatabase()
        {
            //Arrange
            await using var context = new MockDb().CreateDbContext();

            context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
            context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });

            await context.SaveChangesAsync();

            //Act
            var result = await ProjectEndpoints.DeleteProject(1, context);

            //Assert
            Assert.IsType<Results<NoContent, NotFound>>(result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Empty(context.Tasks);
        }
    }
}
