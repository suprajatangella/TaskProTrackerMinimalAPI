using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
            taskItems.MapGet("/", GetAllTasks)
                .WithSummary("Gets the list of task items");
            taskItems.MapGet("/complete", GetCompletedTasks)
                 .WithSummary("Gets the list of completed task items");
            taskItems.MapGet("/{id}", GetTask)
                .WithSummary("Gets the task item by id");
            taskItems.MapPost("/", CreateTask)
                .WithSummary("creates task")
                .AddEndpointFilter<ValidationFilter<TaskItemDTO>>()
                .RequireAuthorization();
            taskItems.MapPut("/{id}", UpdateTask)
                .AddEndpointFilter<ValidationFilter<TaskItemDTO>>()
                .WithSummary("updates task")
                .RequireAuthorization();
            taskItems.MapDelete("/{id}", DeleteTask).WithSummary("deletes task")
                .RequireAuthorization();
        }

        public static async Task<Results<Ok<List<TaskItem>>, NotFound>> GetAllTasks(AppDbContext db)
        {
            var tasks = await db.Tasks.ToListAsync();
            return tasks.Count > 0 ? TypedResults.Ok(tasks) : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<List<TaskItem>>, NotFound>> GetCompletedTasks(AppDbContext db)
        {
            var completedTasks = await db.Tasks.Where(t => t.IsCompleted).ToListAsync();
            return completedTasks.Count > 0 ? TypedResults.Ok(completedTasks) : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<TaskItem>, NotFound>> GetTask(int id, AppDbContext db)
        {
            var task = await db.Tasks.FindAsync(id);
            return task is not null ? TypedResults.Ok(task) : TypedResults.NotFound();
        }

        public static async Task<Created<TaskItem>> CreateTask(TaskItemDTO taskItemDTO, AppDbContext db)
        {
            var taskItem = new TaskItem
            {
                IsCompleted = taskItemDTO.IsCompleted,
                Title = taskItemDTO.Title,
                ProjectId = taskItemDTO.ProjectId
            };

            db.Tasks.Add(taskItem);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/taskitems/{taskItem.Id}", taskItem);
        }

        public static async Task<Results<Created<TaskItem>, NotFound>> UpdateTask(int id, TaskItemDTO taskItemDTO, AppDbContext db)
        {
            var task = await db.Tasks.FindAsync(id);

            if (task is null) return TypedResults.NotFound();

            task.Title = taskItemDTO.Title;
            task.IsCompleted = taskItemDTO.IsCompleted;
            task.ProjectId = taskItemDTO.ProjectId;

            db.Tasks.Update(task);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/tasks/{task.Id}", task);
        }

        public static async Task<Results<NoContent, NotFound>> DeleteTask(int id, AppDbContext db)
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
