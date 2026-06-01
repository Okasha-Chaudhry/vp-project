using StudyConnect.API.Dtos;

namespace StudyConnect.API.Services.Interfaces;

public interface ITaskService
{
    Task<GroupTaskDto> CreateTaskAsync(Guid userId, Guid groupId, CreateTaskRequest request);
    Task<GroupTaskDto?> UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskRequest request);
    Task DeleteTaskAsync(Guid taskId, Guid userId);
    Task<IEnumerable<GroupTaskDto>> GetGroupTasksAsync(Guid groupId, Guid userId);
    Task<IEnumerable<GroupTaskDto>> GetUserTasksAsync(Guid userId);
    Task<GroupTaskDto?> GetTaskByIdAsync(Guid taskId);
}
