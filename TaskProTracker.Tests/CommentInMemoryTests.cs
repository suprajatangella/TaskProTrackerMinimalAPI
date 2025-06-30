using Microsoft.AspNetCore.Http.HttpResults;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Endpoints;
using TaskProTracker.MinimalAPI.Models;
using TaskProTracker.Tests.Helpers;

namespace TaskProTracker.Tests
{
    [Collection("Logger Collection")]
    public class CommentInMemoryTests
    {
        private readonly Serilog.ILogger _log;
        public CommentInMemoryTests(LoggerFixture log)
        {
            _log = log.Logger;
        }
        [Fact]
        public async Task GetCommentReturnsNotFoundIfNotExists()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                // Act
                var result = await CommentEndpoints.GetComment(1, context);

                //Assert
                Assert.IsType<Results<Ok<Comment>, NotFound>>(result);

                var notFoundResult = (NotFound)result.Result;

                Assert.NotNull(notFoundResult);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetCommentReturnsNotFoundIfNotExists test method");
                throw;
            }

        }
        [Fact]
        public async Task GetAllReturnsCommentsFromDatabase()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                context.Comments.Add(new Comment { Id = 1, Content = "Finished unit testing of API 1", TaskItemId = 1, UserId = 1 });
                context.Comments.Add(new Comment { Id = 2, Content = "Finished unit testing of API 2", TaskItemId = 1, UserId = 1 });

                await context.SaveChangesAsync();

                // Act
                var result = await CommentEndpoints.GetAllComments(context);

                //Assert
                Assert.IsType<Results<Ok<List<Comment>>, NotFound>>(result);

                var okResult = (Ok<List<Comment>>)(result.Result); // Cast the Results
                var Comments = Assert.IsAssignableFrom<IEnumerable<Comment>>(okResult.Value); // Get the data

                Assert.NotEmpty(Comments); // Now this works, because 'todos' is IEnumerable
                Assert.Collection(Comments,
                    Comment1 =>
                    {
                        Assert.Equal(1, Comment1.Id);
                        Assert.Equal("Finished unit testing of API 1", Comment1.Content);
                        Assert.Equal(1, Comment1.UserId);
                    },
                    Comment2 =>
                    {
                        Assert.Equal(2, Comment2.Id);
                        Assert.Equal("Finished unit testing of API 2", Comment2.Content);
                        Assert.Equal(1, Comment2.UserId);
                    }
                );
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetAllReturnsCommentsFromDatabase test method");
                throw;
            }

        }
        [Fact]
        public async Task GetCommentReturnsCommentFromDatabase()
        {
            try
            {
                // Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                context.Comments.Add(new Comment { Id = 1, Content = "Finished unit testing of API 1", TaskItemId = 1, UserId = 1 });

                await context.SaveChangesAsync();

                // Act
                var result = await CommentEndpoints.GetComment(1, context);

                //Assert
                Assert.IsType<Results<Ok<Comment>, NotFound>>(result);

                var okResult = (Ok<Comment>)result.Result;

                Assert.NotNull(okResult.Value);
                Assert.Equal(1, okResult.Value.Id);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in GetCommentReturnsCommentFromDatabase test method");
                throw;
            }
        }
        [Fact]
        public async Task CreateCommentCreatesCommentInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                var commentDto = new CommentDto { Content = "Test Content 1", TaskItemId = 1, UserId = 1 };

                //Act
                var result = await CommentEndpoints.CreateComment(commentDto, context);

                //Assert
                Assert.IsType<Created<Comment>>(result);

                Assert.NotNull(result);
                Assert.NotNull(result.Location);

                Assert.NotEmpty(context.Comments);
                Assert.Collection(context.Comments, Comment =>
                {
                    Assert.Equal("Test Content 1", Comment.Content);
                    Assert.Equal(1, Comment.Id);
                    Assert.Equal(1, Comment.UserId);
                });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in CreateCommentCreatesCommentInDatabase test method");
                throw;
            }
        }

        [Fact]
        public async Task UpdateCommentUpdatesCommentInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                context.Comments.Add(new Comment { Id = 1, Content = "Test Content", UserId = 1, TaskItemId = 1 });

                await context.SaveChangesAsync();

                var updatedComment = new CommentDto
                {
                    Id = 1,
                    Content = "Updated Test Content",
                    UserId = 1,
                    TaskItemId = 1
                };

                //Act
                var result = await CommentEndpoints.UpdateComment(updatedComment.Id, updatedComment, context);

                //Assert
                Assert.IsType<Results<Created<Comment>, NotFound>>(result);

                var createdResult = (Created<Comment>)result.Result;

                Assert.NotNull(createdResult);
                Assert.NotNull(createdResult.Location);

                var commentInDb = await context.Comments.FindAsync(1);

                Assert.NotNull(commentInDb);
                Assert.Equal("Updated Test Content", commentInDb!.Content);
                Assert.Equal(1, commentInDb.UserId);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in UpdateCommentUpdatesCommentInDatabase test method");
                throw;
            }
        }

        [Fact]
        public async Task DeleteCommentDeletesCommentInDatabase()
        {
            try
            {
                //Arrange
                await using var context = new MockDb().CreateDbContext();

                context.Users.Add(new User { Id = 1, Name = "Admin", Email = "Admin@gmail.com", PasswordHash = "Admin@123", Role = "Admin" });
                context.Projects.Add(new Project { Id = 1, Title = "Test Project", Description = "Creating Unit tests", UserId = 1 });
                context.Tasks.Add(new TaskItem { Id = 1, Title = "Test title 1", ProjectId = 1, IsCompleted = false });
                context.Comments.Add(new Comment { Id = 1, Content = "Test Content", TaskItemId = 1, UserId = 1 });

                await context.SaveChangesAsync();

                //Act
                var result = await CommentEndpoints.DeleteComment(1, context);

                //Assert
                Assert.IsType<Results<NoContent, NotFound>>(result);

                var noContentResult = (NoContent)result.Result;

                Assert.NotNull(noContentResult);
                Assert.Empty(context.Comments);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error in DeleteCommentDeletesCommentInDatabase test method");
                throw;
            }
        }
    }
}
