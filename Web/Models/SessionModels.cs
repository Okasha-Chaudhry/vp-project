using System.ComponentModel.DataAnnotations;

namespace StudyConnect.Web.Models;

public class CreateSessionRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Schedule date is required")]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes")]
    public int DurationMinutes { get; set; } = 60;

    [StringLength(500, ErrorMessage = "Meeting link cannot exceed 500 characters")]
    public string? MeetingLink { get; set; }

    public bool UseBuiltInVideoCall { get; set; } = true; // Default to built-in Jitsi
}

public class UpdateSessionRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Schedule date is required")]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes")]
    public int DurationMinutes { get; set; } = 60;

    [StringLength(500, ErrorMessage = "Meeting link cannot exceed 500 characters")]
    public string? MeetingLink { get; set; }

    public bool UseBuiltInVideoCall { get; set; } = true;
}

public class SessionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? MeetingLink { get; set; }
    public string RoomName { get; set; } = string.Empty; // Jitsi room name
    public string? VideoCallUrl { get; set; } // Full Jitsi URL
    public bool UseBuiltInVideoCall { get; set; } = true;
    public UserDto CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsUpcoming { get; set; }
    public bool IsActive { get; set; } // True if session is currently active
}

// Legacy alias for backward compatibility
public class StudySessionDto : SessionDto { }

