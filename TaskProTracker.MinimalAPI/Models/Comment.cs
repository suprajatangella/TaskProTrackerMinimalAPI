using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProTracker.MinimalAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public int TaskItemId { get; set; }

        [NotMapped]
        public TaskItem? TaskItem { get; set; }

        public int UserId { get; set; }
        [NotMapped]
        public User? User { get; set; }
    }
}
