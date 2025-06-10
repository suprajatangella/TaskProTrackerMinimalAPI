using System.Xml.Linq;

namespace TaskProTracker.MinimalAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
