using System.ComponentModel.DataAnnotations;

namespace StudyConnect.API.Models;

public enum ResourceType
{
    Note,
    Pdf,
    Video,
    Link
}

public class SharedResource
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public ResourceType Type { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public Guid GroupId { get; set; }
    public StudyGroup Group { get; set; } = null!;

    public Guid UploadedByUserId { get; set; }
    public User UploadedBy { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
