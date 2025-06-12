using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Dtos
{
    public class TaskItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int CommentId { get; set; }

        public TaskItemDTO() { }
        public TaskItemDTO(TaskItem taskItem) =>
        (Id, Title, IsCompleted, CommentId) = (taskItem.Id, taskItem.Title, taskItem.IsCompleted, taskItem.CommentId);
    }
}
