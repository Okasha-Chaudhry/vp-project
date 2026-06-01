using StudyConnect.API.Dtos;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;
using StudyConnect.API.Services.Interfaces;

namespace StudyConnect.API.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IGroupRepository _groupRepository;

    public ResourceService(IResourceRepository resourceRepository, IGroupRepository groupRepository)
    {
        _resourceRepository = resourceRepository;
        _groupRepository = groupRepository;
    }

    public async Task<SharedResourceDto> CreateResourceAsync(Guid userId, Guid groupId, CreateResourceRequest request)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        var resource = new SharedResource
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Content = request.Content,
            GroupId = groupId,
            UploadedByUserId = userId
        };

        var created = await _resourceRepository.CreateAsync(resource);
        var withUser = await _resourceRepository.GetByIdAsync(created.Id);
        return MapToDto(withUser!);
    }

    public async Task<SharedResourceDto?> UpdateResourceAsync(Guid resourceId, Guid userId, UpdateResourceRequest request)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId);
        if (resource == null) return null;
        if (resource.UploadedByUserId != userId) throw new UnauthorizedAccessException("You can only edit your own resources");

        resource.Title = request.Title;
        resource.Description = request.Description;
        resource.Content = request.Content;

        await _resourceRepository.UpdateAsync(resource);

        var updated = await _resourceRepository.GetByIdAsync(resourceId);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task DeleteResourceAsync(Guid resourceId, Guid userId)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId);
        if (resource == null) throw new KeyNotFoundException("Resource not found");
        if (resource.UploadedByUserId != userId) throw new UnauthorizedAccessException("You can only delete your own resources");

        await _resourceRepository.DeleteAsync(resourceId);
    }

    public async Task<IEnumerable<SharedResourceDto>> GetGroupResourcesAsync(Guid groupId, Guid userId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        var resources = await _resourceRepository.GetByGroupAsync(groupId);
        return resources.Select(MapToDto);
    }

    public async Task<SharedResourceDto?> GetResourceByIdAsync(Guid resourceId)
    {
        var resource = await _resourceRepository.GetByIdAsync(resourceId);
        return resource == null ? null : MapToDto(resource);
    }

    private static SharedResourceDto MapToDto(SharedResource resource)
    {
        return new SharedResourceDto
        {
            Id = resource.Id,
            Title = resource.Title,
            Description = resource.Description,
            Type = resource.Type.ToString(),
            Content = resource.Content,
            UploadedBy = new UserDto
            {
                Id = resource.UploadedBy.Id,
                Email = resource.UploadedBy.Email,
                Name = resource.UploadedBy.Name,
                AvatarUrl = resource.UploadedBy.AvatarUrl,
                CreatedAt = resource.UploadedBy.CreatedAt
            },
            UploadedAt = resource.UploadedAt
        };
    }
}
