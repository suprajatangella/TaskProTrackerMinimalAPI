using Microsoft.AspNetCore.Http.HttpResults;
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
            RouteGroupBuilder Project = app.MapGroup("/projects");
            Project.MapGet("/", GetAllProjects);
            Project.MapGet("/{id}", GetProject);
            Project.MapPost("/", CreateProject)
                .AddEndpointFilter<ValidationFilter<ProjectDto>>()
                .RequireAuthorization();
            Project.MapPut("/{id}", UpdateProject)
                .AddEndpointFilter<ValidationFilter<ProjectDto>>()
                .RequireAuthorization();
            Project.MapDelete("/{id}", DeleteProject).RequireAuthorization();
        }

        public static async Task<Results<Ok<List<Project>>, NotFound>> GetAllProjects(AppDbContext db)
        {
            var projects = await db.Projects.Include(p => p.User).ToListAsync();
            return projects.Count > 0 ? TypedResults.Ok(projects) : TypedResults.NotFound();
        }
        public static async Task<Results<Ok<Project>, NotFound>> GetProject(int id, AppDbContext db)
        {
            var project = await db.Projects.FindAsync(id);
            return project is not null ? TypedResults.Ok(project) : TypedResults.NotFound();
        }

        public static async Task<Created<Project>> CreateProject(ProjectDto projDto, AppDbContext db)
        {
            var project = new Project
            {
                Title = projDto.Title,
                Description = projDto.Description,
                UserId = projDto.UserId
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/projects/{project.Id}", project);
        }

        public static async Task<Results<Created<Project>, NotFound>> UpdateProject(int id, ProjectDto projDto, AppDbContext db)
        {
            var existingProj = await db.Projects.FindAsync(id);

            if (existingProj is null)
                return TypedResults.NotFound();
            else
            {
                existingProj.Description = !string.IsNullOrEmpty(projDto.Description) ? projDto.Description : existingProj.Description;
                existingProj.Title = !string.IsNullOrEmpty(projDto.Title) ? projDto.Title : existingProj.Title;
                existingProj.UserId = (projDto.UserId > 0) ? projDto.UserId : existingProj.UserId;
            }

            db.Projects.Update(existingProj);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/projects/{existingProj.Id}", existingProj);
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
