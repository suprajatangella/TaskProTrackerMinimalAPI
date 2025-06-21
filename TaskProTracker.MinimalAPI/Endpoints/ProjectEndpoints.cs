using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Endpoints
{
    public static class ProjectEndpoints
    {
        public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder Project = app.MapGroup("/projects");
            Project.MapGet("/", GetAllProjects);
            Project.MapGet("/{id}", GetProject);
            Project.MapPost("/", CreateProject).RequireAuthorization();
            Project.MapPut("/{id}", UpdateProject).RequireAuthorization();
            Project.MapDelete("/{id}", DeleteProject).RequireAuthorization();
        }

        public static async Task<Results<Ok<List<Project>>, NotFound>> GetAllProjects(AppDbContext db)
        {
            var projects = await db.Projects.Include( p => p.User ).ToListAsync();
            return projects.Count > 0 ? TypedResults.Ok(projects) : TypedResults.NotFound();
        }
        public static async Task<Results<Ok<Project>, NotFound>> GetProject(int id, AppDbContext db)
        {
            var project = await db.Projects.FindAsync(id);
            return project is not null ? TypedResults.Ok(project) : TypedResults.NotFound();
        }

        public static async Task<Created<Project>> CreateProject(Project proj, AppDbContext db)
        {
            db.Projects.Add(proj);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/projects/{proj.Id}", proj);
        }

        public static async Task<Results<Created<Project>, NotFound>> UpdateProject(int id, Project proj, AppDbContext db)
        {
            var existingProj = await db.Projects.FindAsync(id);

            if (existingProj is null) return TypedResults.NotFound();

            db.Projects.Update(proj);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/projects/{proj.Id}", proj);
        }

        public static async Task<Results<NoContent, NotFound>> DeleteProject(int id, AppDbContext db)
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
