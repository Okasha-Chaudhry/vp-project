using StudyConnect.API.Models;

namespace StudyConnect.API.Repositories.Interfaces;

public interface IResourceRepository
{
    Task<SharedResource?> GetByIdAsync(Guid id);
    Task<IEnumerable<SharedResource>> GetByGroupAsync(Guid groupId);
    Task<SharedResource> CreateAsync(SharedResource resource);
    Task UpdateAsync(SharedResource resource);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByGroupAsync(Guid groupId);
}
