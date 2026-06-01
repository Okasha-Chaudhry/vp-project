using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public interface ITaskService
{
    Task<List<GroupTaskDto>> GetGroupTasksAsync(Guid groupId);
    Task<List<GroupTaskDto>> GetMyTasksAsync();
    Task<GroupTaskDto?> CreateTaskAsync(Guid groupId, CreateTaskRequest request);
    Task<GroupTaskDto?> UpdateTaskAsync(Guid taskId, Guid groupId, UpdateTaskRequest request);
    Task<bool> DeleteTaskAsync(Guid taskId, Guid groupId);
}
