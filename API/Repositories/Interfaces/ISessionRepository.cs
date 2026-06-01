using StudyConnect.API.Models;

namespace StudyConnect.API.Repositories.Interfaces;

public interface ISessionRepository
{
    Task<StudySession?> GetByIdAsync(Guid id);
    Task<IEnumerable<StudySession>> GetByGroupAsync(Guid groupId);
    Task<StudySession> CreateAsync(StudySession session);
    Task UpdateAsync(StudySession session);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByGroupAsync(Guid groupId);
}
