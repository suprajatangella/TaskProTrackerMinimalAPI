using System.ComponentModel.DataAnnotations;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId should be > 0")]
        public int UserId { get; set; }

        public ProjectDto() { }
        public ProjectDto(Project project) =>
        (Id, Title, Description, UserId) = (project.Id, project.Title, project.Description, project.UserId);
    }
}
