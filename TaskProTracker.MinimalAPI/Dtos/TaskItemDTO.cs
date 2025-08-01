﻿using System.ComponentModel.DataAnnotations;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Dtos
{
    public class TaskItemDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;

        [Range(1, int.MaxValue, ErrorMessage = "ProjectId should be > 0")]
        public int ProjectId { get; set; }

        public TaskItemDTO() { }
        public TaskItemDTO(TaskItem taskItem) =>
        (Id, Title, IsCompleted, ProjectId) = (taskItem.Id, taskItem.Title, taskItem.IsCompleted, taskItem.ProjectId);

    }
}
