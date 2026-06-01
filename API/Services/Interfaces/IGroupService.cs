using StudyConnect.API.Dtos;

namespace StudyConnect.API.Services.Interfaces;

public interface IGroupService
{
    Task<StudyGroupDto> CreateGroupAsync(Guid userId, CreateGroupRequest request);
    Task<StudyGroupDto?> JoinGroupAsync(Guid userId, JoinGroupRequest request);
    Task<GroupDetailDto?> GetGroupDetailAsync(Guid groupId, Guid userId);
    Task<IEnumerable<StudyGroupDto>> GetUserGroupsAsync(Guid userId);
    Task<IEnumerable<StudyGroupDto>> GetAllGroupsAsync();
    Task LeaveGroupAsync(Guid groupId, Guid userId);
    Task DeleteGroupAsync(Guid groupId, Guid userId);
}
