using System.Xml.Linq;

namespace TaskProTracker.MinimalAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
