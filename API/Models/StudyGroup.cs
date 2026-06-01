using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Models;

public class StudyGroup
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string InviteCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

    public Guid CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<SharedResource> Resources { get; set; } = new List<SharedResource>();
    public ICollection<StudySession> Sessions { get; set; } = new List<StudySession>();
    public ICollection<GroupTask> Tasks { get; set; } = new List<GroupTask>();
}
