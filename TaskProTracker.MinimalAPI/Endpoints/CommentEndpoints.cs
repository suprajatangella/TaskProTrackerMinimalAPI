using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
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

        static async Task<IResult> GetAllComments(AppDbContext db)
        {
            return TypedResults.Ok(await db.Comments.Include(x=>x.User).ToListAsync());
        }

        //static async Task<IResult> GetCompletedTasks(AppDbContext db)
        //{
        //    return TypedResults.Ok(await db.Tasks.Where(t => t.IsCompleted).Select(x => new TaskItemDTO(x)).ToListAsync());
        //}

        static async Task<IResult> GetComment(int id, AppDbContext db)
        {
            return await db.Comments.FindAsync(id)
                is Comment Comment
                    ? TypedResults.Ok(Comment)
                    : TypedResults.NotFound();
        }

        static async Task<IResult> CreateComment(Comment proj, AppDbContext db)
        {
            db.Comments.Add(proj);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/Comments/{proj.Id}", proj);
        }

        static async Task<IResult> UpdateComment(int id, Comment proj, AppDbContext db)
        {
            var existingProj = await db.Comments.FindAsync(id);

            if (existingProj is null) return TypedResults.NotFound();

            db.Comments.Update(proj);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteComment(int id, AppDbContext db)
        {
            if (await db.Comments.FindAsync(id) is Comment proj)
            {
                db.Comments.Remove(proj);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

    }
}
