using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProTracker.MinimalAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int CommentId { get; set; }
        //[NotMapped]
        public Comment? Comment { get; set; }
        //[NotMapped]
        //public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
