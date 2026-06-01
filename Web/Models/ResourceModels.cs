using System.ComponentModel.DataAnnotations;

namespace StudyConnect.Web.Models;

public enum ResourceType
{
    Note,
    Pdf,
    Video,
    Link
}

public class CreateResourceRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Resource type is required")]
    public ResourceType Type { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
}

public class UpdateResourceRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
}

public class SharedResourceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public UserDto UploadedBy { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}
