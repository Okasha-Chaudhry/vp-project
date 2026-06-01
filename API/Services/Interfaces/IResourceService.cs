using StudyConnect.API.Dtos;

namespace StudyConnect.API.Services.Interfaces;

public interface IResourceService
{
    Task<SharedResourceDto> CreateResourceAsync(Guid userId, Guid groupId, CreateResourceRequest request);
    Task<SharedResourceDto?> UpdateResourceAsync(Guid resourceId, Guid userId, UpdateResourceRequest request);
    Task DeleteResourceAsync(Guid resourceId, Guid userId);
    Task<IEnumerable<SharedResourceDto>> GetGroupResourcesAsync(Guid groupId, Guid userId);
    Task<SharedResourceDto?> GetResourceByIdAsync(Guid resourceId);
}
