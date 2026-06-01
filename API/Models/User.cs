using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<StudyGroup> CreatedGroups { get; set; } = new List<StudyGroup>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public ICollection<SharedResource> UploadedResources { get; set; } = new List<SharedResource>();
    public ICollection<StudySession> CreatedSessions { get; set; } = new List<StudySession>();
    public ICollection<GroupTask> AssignedTasks { get; set; } = new List<GroupTask>();
}
