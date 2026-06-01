using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public interface IResourceService
{
    Task<List<SharedResourceDto>> GetGroupResourcesAsync(Guid groupId);
    Task<SharedResourceDto?> CreateResourceAsync(Guid groupId, CreateResourceRequest request);
    Task<SharedResourceDto?> UpdateResourceAsync(Guid resourceId, Guid groupId, UpdateResourceRequest request);
    Task<bool> DeleteResourceAsync(Guid resourceId, Guid groupId);
}
