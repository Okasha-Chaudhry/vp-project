using StudyConnect.API.Dtos;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;
using StudyConnect.API.Services.Interfaces;
using TaskStatus = StudyConnect.API.Models.TaskStatus;
namespace StudyConnect.API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IGroupRepository _groupRepository;

    public TaskService(ITaskRepository taskRepository, IGroupRepository groupRepository)
    {
        _taskRepository = taskRepository;
        _groupRepository = groupRepository;
    }

    public async Task<GroupTaskDto> CreateTaskAsync(Guid userId, Guid groupId, CreateTaskRequest request)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        var task = new GroupTask
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate?.ToUniversalTime(),
            GroupId = groupId,
            AssignedToUserId = request.AssignedToUserId,
            CreatedByUserId = userId
        };

        var created = await _taskRepository.CreateAsync(task);
        var withRelations = await _taskRepository.GetByIdAsync(created.Id);
        return MapToDto(withRelations!);
    }

    public async Task<GroupTaskDto?> UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) return null;
        if (task.CreatedByUserId != userId && task.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("You can only edit tasks you created or are assigned to");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate?.ToUniversalTime();
        task.AssignedToUserId = request.AssignedToUserId;

        await _taskRepository.UpdateAsync(task);

        var updated = await _taskRepository.GetByIdAsync(taskId);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Task not found");
        if (task.CreatedByUserId != userId) throw new UnauthorizedAccessException("You can only delete tasks you created");

        await _taskRepository.DeleteAsync(taskId);
    }

    public async Task<IEnumerable<GroupTaskDto>> GetGroupTasksAsync(Guid groupId, Guid userId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        var tasks = await _taskRepository.GetByGroupAsync(groupId);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<GroupTaskDto>> GetUserTasksAsync(Guid userId)
    {
        var tasks = await _taskRepository.GetByUserAsync(userId);
        return tasks.Select(MapToDto);
    }

    public async Task<GroupTaskDto?> GetTaskByIdAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        return task == null ? null : MapToDto(task);
    }

    private static GroupTaskDto MapToDto(GroupTask task)
    {
        return new GroupTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            AssignedTo = task.AssignedTo == null ? null : new UserDto
            {
                Id = task.AssignedTo.Id,
                Email = task.AssignedTo.Email,
                Name = task.AssignedTo.Name,
                AvatarUrl = task.AssignedTo.AvatarUrl,
                CreatedAt = task.AssignedTo.CreatedAt
            },
            CreatedBy = new UserDto
            {
                Id = task.CreatedBy.Id,
                Email = task.CreatedBy.Email,
                Name = task.CreatedBy.Name,
                AvatarUrl = task.CreatedBy.AvatarUrl,
                CreatedAt = task.CreatedBy.CreatedAt
            },
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
            IsOverdue = task.DueDate.HasValue && task.DueDate.Value < DateTime.UtcNow && task.Status != TaskStatus.Completed
        };
    }
}
