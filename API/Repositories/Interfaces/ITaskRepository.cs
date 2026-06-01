using StudyConnect.API.Models;

namespace StudyConnect.API.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<GroupTask?> GetByIdAsync(Guid id);
    Task<IEnumerable<GroupTask>> GetByGroupAsync(Guid groupId);
    Task<IEnumerable<GroupTask>> GetByUserAsync(Guid userId);
    Task<GroupTask> CreateAsync(GroupTask task);
    Task UpdateAsync(GroupTask task);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByGroupAsync(Guid groupId);
}
