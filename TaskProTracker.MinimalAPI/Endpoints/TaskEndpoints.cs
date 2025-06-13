using Microsoft.EntityFrameworkCore;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder taskItems = app.MapGroup("/tasks");
            taskItems.MapGet("/", GetAllTasks);
            taskItems.MapGet("/complete", GetCompletedTasks);
            taskItems.MapGet("/{id}", GetTask);
            taskItems.MapPost("/", CreateTask).RequireAuthorization();
            taskItems.MapPut("/{id}", UpdateTask).RequireAuthorization();
            taskItems.MapDelete("/{id}", DeleteTask).RequireAuthorization();
        }

        static async Task<IResult> GetAllTasks(AppDbContext db)
        {
            return TypedResults.Ok(await db.Tasks.Select(x => new TaskItemDTO(x)).ToListAsync());
        }

        static async Task<IResult> GetCompletedTasks(AppDbContext db)
        {
            return TypedResults.Ok(await db.Tasks.Where(t => t.IsCompleted).Select(x => new TaskItemDTO(x)).ToListAsync());
        }

        static async Task<IResult> GetTask(int id, AppDbContext db)
        {
            return await db.Tasks.FindAsync(id)
                is TaskItem task
                    ? TypedResults.Ok(new TaskItemDTO(task))
                    : TypedResults.NotFound();
        }

        static async Task<IResult> CreateTask(TaskItemDTO taskItemDTO, AppDbContext db)
        {
            var taskItem = new TaskItem
            {
                IsCompleted = taskItemDTO.IsCompleted,
                Title = taskItemDTO.Title,
                ProjectId = taskItemDTO.ProjectId
            };

            db.Tasks.Add(taskItem);
            await db.SaveChangesAsync();

            taskItemDTO = new TaskItemDTO(taskItem);

            return TypedResults.Created($"/taskitems/{taskItem.Id}", taskItemDTO);
        }

        static async Task<IResult> UpdateTask(int id, TaskItemDTO taskItemDTO, AppDbContext db)
        {
            var task = await db.Tasks.FindAsync(id);

            if (task is null) return TypedResults.NotFound();

            task.Title = taskItemDTO.Title;
            task.IsCompleted = taskItemDTO.IsCompleted;
            task.ProjectId = taskItemDTO.ProjectId;

            db.Tasks.Update(task);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteTask(int id, AppDbContext db)
        {
            if (await db.Tasks.FindAsync(id) is TaskItem task)
            {
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

    }
}
