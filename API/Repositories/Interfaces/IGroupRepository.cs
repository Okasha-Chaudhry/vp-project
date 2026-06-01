using StudyConnect.API.Models;

namespace StudyConnect.API.Repositories.Interfaces;

public interface IGroupRepository
{
    Task<StudyGroup?> GetByIdAsync(Guid id);
    Task<StudyGroup?> GetByInviteCodeAsync(string inviteCode);
    Task<IEnumerable<StudyGroup>> GetAllAsync();
    Task<IEnumerable<StudyGroup>> GetByUserAsync(Guid userId);
    Task<StudyGroup> CreateAsync(StudyGroup group);
    Task UpdateAsync(StudyGroup group);
    Task DeleteAsync(Guid id);
    Task<bool> IsMemberAsync(Guid groupId, Guid userId);
    Task<GroupMember> AddMemberAsync(Guid groupId, Guid userId);
    Task RemoveMemberAsync(Guid groupId, Guid userId);
    Task<int> GetMemberCountAsync(Guid groupId);
}
