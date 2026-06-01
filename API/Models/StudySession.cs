using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Models;

public class StudySession
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 480)]
    public int DurationMinutes { get; set; } = 60;

    [StringLength(500)]
    public string? MeetingLink { get; set; }

    [Required]
    [StringLength(100)]
    public string RoomName { get; set; } = string.Empty; // Jitsi room identifier

    public bool UseBuiltInVideoCall { get; set; } = true; // Default to built-in Jitsi video

    public Guid GroupId { get; set; }
    public StudyGroup Group { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
