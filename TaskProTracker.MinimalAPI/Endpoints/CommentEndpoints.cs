using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Endpoints
{
    public static class CommentEndpoints
    {
        public static void MapCommentEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder comment = app.MapGroup("/comments");
            comment.MapGet("/", GetAllComments);
            comment.MapGet("/{id}", GetComment);
            comment.MapPost("/", CreateComment).RequireAuthorization();
            comment.MapPut("/{id}", UpdateComment).RequireAuthorization();
            comment.MapDelete("/{id}", DeleteComment).RequireAuthorization();
        }

        public static async Task<Results<Ok<List<Comment>>, NotFound>> GetAllComments(AppDbContext db)
        {
            var comments = await db.Comments.ToListAsync();
            return comments.Count > 0 ? TypedResults.Ok(comments) : TypedResults.NotFound();
        }
        public static async Task<Results<Ok<Comment>, NotFound>> GetComment(int id, AppDbContext db)
        {
            var comment = await db.Comments.FindAsync(id);
            return comment is not null ? TypedResults.Ok(comment) : TypedResults.NotFound();
        }

        public static async Task<Created<Comment>> CreateComment(Comment comment, AppDbContext db)
        {
            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/comments/{comment.Id}", comment);
        }

        public static async Task<Results<Created<Comment>, NotFound>> UpdateComment(int id, Comment comment, AppDbContext db)
        {
            var existingComment = await db.Comments.FindAsync(id);

            if (existingComment is null) return TypedResults.NotFound();
            // Update the existing comment with the new values
            existingComment.Content = comment.Content;
            existingComment.TaskItemId = comment.TaskItemId;
            existingComment.UserId = comment.UserId;
            // Save the changes to the database

            db.Comments.Update(existingComment);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/comments/{comment.Id}", comment);
        }

        public static async Task<Results<NoContent, NotFound>> DeleteComment(int id, AppDbContext db)
        {
            if (await db.Comments.FindAsync(id) is Comment comment)
            {
                db.Comments.Remove(comment);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

    }
}
