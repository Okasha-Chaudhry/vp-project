using System.ComponentModel.DataAnnotations;
using StudyConnect.API.Models;
using TaskStatus = StudyConnect.API.Models.TaskStatus;

namespace StudyConnect.API.Dtos;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Priority is required")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public Guid? AssignedToUserId { get; set; }
}

public class UpdateTaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public TaskStatus Status { get; set; }

    [Required(ErrorMessage = "Priority is required")]
    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? AssignedToUserId { get; set; }
}

public class GroupTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public UserDto? AssignedTo { get; set; }
    public UserDto CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsOverdue { get; set; }
}