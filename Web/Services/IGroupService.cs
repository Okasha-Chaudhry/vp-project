using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public interface IGroupService
{
    Task<List<StudyGroupDto>> GetMyGroupsAsync();
    Task<StudyGroupDto?> CreateGroupAsync(CreateGroupRequest request);
    Task<StudyGroupDto?> JoinGroupAsync(JoinGroupRequest request);
    Task<GroupDetailDto?> GetGroupDetailAsync(Guid groupId);
    Task<bool> LeaveGroupAsync(Guid groupId);
    Task<bool> DeleteGroupAsync(Guid groupId);
}
