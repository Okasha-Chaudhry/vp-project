using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public interface ISessionService
{
    Task<List<SessionDto>> GetGroupSessionsAsync(Guid groupId);
    Task<SessionDto?> GetSessionByIdAsync(Guid sessionId);
    Task<SessionDto?> CreateSessionAsync(Guid groupId, CreateSessionRequest request);
    Task<SessionDto?> UpdateSessionAsync(Guid sessionId, Guid groupId, UpdateSessionRequest request);
    Task<bool> DeleteSessionAsync(Guid sessionId, Guid groupId);
}
