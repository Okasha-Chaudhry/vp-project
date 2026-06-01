using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Models;

public enum MemberRole
{
    Member,
    Admin
}

public class GroupMember
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }
    public StudyGroup Group { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public MemberRole Role { get; set; } = MemberRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
