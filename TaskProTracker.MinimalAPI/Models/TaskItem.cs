using System.Xml.Linq;

namespace TaskProTracker.MinimalAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
