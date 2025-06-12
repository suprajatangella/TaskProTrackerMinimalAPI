using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder Project = app.MapGroup("/Projects");
            Project.MapGet("/", GetAllProjects);
            Project.MapGet("/{id}", GetProject);
            Project.MapPost("/", CreateProject).RequireAuthorization();
            Project.MapPut("/{id}", UpdateProject).RequireAuthorization();
            Project.MapDelete("/{id}", DeleteProject).RequireAuthorization();
        }

        static async Task<IResult> GetAllProjects(AppDbContext db)
        {
            return TypedResults.Ok(await db.Projects.Include(x=>x.User).ToListAsync());
        }

        //static async Task<IResult> GetCompletedTasks(AppDbContext db)
        //{
        //    return TypedResults.Ok(await db.Tasks.Where(t => t.IsCompleted).Select(x => new TaskItemDTO(x)).ToListAsync());
        //}

        static async Task<IResult> GetProject(int id, AppDbContext db)
        {
            return await db.Projects.FindAsync(id)
                is Project Project
                    ? TypedResults.Ok(Project)
                    : TypedResults.NotFound();
        }

        static async Task<IResult> CreateProject(Project proj, AppDbContext db)
        {
            db.Projects.Add(proj);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/Projects/{proj.Id}", proj);
        }

        static async Task<IResult> UpdateProject(int id, Project proj, AppDbContext db)
        {
            var existingProj = await db.Projects.FindAsync(id);

            if (existingProj is null) return TypedResults.NotFound();

            db.Projects.Update(proj);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteProject(int id, AppDbContext db)
        {
            if (await db.Projects.FindAsync(id) is Project proj)
            {
                db.Projects.Remove(proj);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

    }
}
