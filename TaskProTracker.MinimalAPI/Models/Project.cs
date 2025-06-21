using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProTracker.MinimalAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        //[NotMapped]
        public User? User { get; set; }
        [NotMapped]
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
