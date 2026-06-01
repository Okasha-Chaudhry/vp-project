using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Dtos;

public class CreateGroupRequest
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}

public class JoinGroupRequest
{
    [Required(ErrorMessage = "Invite code is required")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Invite code must be exactly 8 characters")]
    public string InviteCode { get; set; } = string.Empty;
}

public class StudyGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string InviteCode { get; set; } = string.Empty;
    public UserDto CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public int ResourceCount { get; set; }
    public int SessionCount { get; set; }
    public int TaskCount { get; set; }
    public bool IsMember { get; set; }
    public string UserRole { get; set; } = string.Empty;
}

public class GroupDetailDto : StudyGroupDto
{
    public List<GroupMemberDto> Members { get; set; } = new();
    public List<SharedResourceDto> Resources { get; set; } = new();
    public List<StudySessionDto> Sessions { get; set; } = new();
    public List<GroupTaskDto> Tasks { get; set; } = new();
}

public class GroupMemberDto
{
    public Guid Id { get; set; }
    public UserDto User { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}
