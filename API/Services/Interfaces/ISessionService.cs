using StudyConnect.API.Dtos;

namespace StudyConnect.API.Services.Interfaces;

public interface ISessionService
{
    Task<StudySessionDto> CreateSessionAsync(Guid userId, Guid groupId, CreateSessionRequest request);
    Task<StudySessionDto?> UpdateSessionAsync(Guid sessionId, Guid userId, UpdateSessionRequest request);
    Task DeleteSessionAsync(Guid sessionId, Guid userId);
    Task<IEnumerable<StudySessionDto>> GetGroupSessionsAsync(Guid groupId, Guid userId);
    Task<StudySessionDto?> GetSessionByIdAsync(Guid sessionId);
}
