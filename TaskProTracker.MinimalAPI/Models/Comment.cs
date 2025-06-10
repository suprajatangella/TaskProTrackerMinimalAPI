namespace TaskProTracker.MinimalAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
