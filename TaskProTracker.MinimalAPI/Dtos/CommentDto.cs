using System.ComponentModel.DataAnnotations;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Dtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "TaskItemId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TaskItemId should be > 0")]
        public int TaskItemId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId should be > 0")]
        public int UserId { get; set; }

        public CommentDto() { }
        public CommentDto(Comment comment) =>
        (Id, Content, TaskItemId, UserId) = (comment.Id, comment.Content, comment.TaskItemId, comment.UserId);
    }
}
