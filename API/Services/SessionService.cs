using StudyConnect.API.Dtos;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;
using StudyConnect.API.Services.Interfaces;

namespace StudyConnect.API.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IConfiguration _configuration;

    public SessionService(ISessionRepository sessionRepository, IGroupRepository groupRepository, IConfiguration configuration)
    {
        _sessionRepository = sessionRepository;
        _groupRepository = groupRepository;
        _configuration = configuration;
    }

    public async Task<StudySessionDto> CreateSessionAsync(Guid userId, Guid groupId, CreateSessionRequest request)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        // Generate unique room name for Jitsi
        var roomName = GenerateRoomName(groupId);

        var session = new StudySession
        {
            Title = request.Title,
            Description = request.Description,
            ScheduledAt = request.ScheduledAt.ToUniversalTime(),
            DurationMinutes = request.DurationMinutes,
            MeetingLink = request.MeetingLink,
            RoomName = roomName,
            UseBuiltInVideoCall = request.UseBuiltInVideoCall,
            GroupId = groupId,
            CreatedByUserId = userId
        };

        var created = await _sessionRepository.CreateAsync(session);
        var withUser = await _sessionRepository.GetByIdAsync(created.Id);
        return MapToDto(withUser!);
    }

    public async Task<StudySessionDto?> UpdateSessionAsync(Guid sessionId, Guid userId, UpdateSessionRequest request)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) return null;
        if (session.CreatedByUserId != userId) throw new UnauthorizedAccessException("You can only edit your own sessions");

        session.Title = request.Title;
        session.Description = request.Description;
        session.ScheduledAt = request.ScheduledAt.ToUniversalTime();
        session.DurationMinutes = request.DurationMinutes;
        session.MeetingLink = request.MeetingLink;
        session.UseBuiltInVideoCall = request.UseBuiltInVideoCall;

        await _sessionRepository.UpdateAsync(session);

        var updated = await _sessionRepository.GetByIdAsync(sessionId);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task DeleteSessionAsync(Guid sessionId, Guid userId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) throw new KeyNotFoundException("Session not found");
        if (session.CreatedByUserId != userId) throw new UnauthorizedAccessException("You can only delete your own sessions");

        await _sessionRepository.DeleteAsync(sessionId);
    }

    public async Task<IEnumerable<StudySessionDto>> GetGroupSessionsAsync(Guid groupId, Guid userId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) throw new UnauthorizedAccessException("You are not a member of this group");

        var sessions = await _sessionRepository.GetByGroupAsync(groupId);
        return sessions.Select(MapToDto);
    }

    public async Task<StudySessionDto?> GetSessionByIdAsync(Guid sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        return session == null ? null : MapToDto(session);
    }

    private StudySessionDto MapToDto(StudySession session)
    {
        var isUpcoming = session.ScheduledAt > DateTime.UtcNow;
        var sessionEndTime = session.ScheduledAt.AddMinutes(session.DurationMinutes);
        var isActive = DateTime.UtcNow >= session.ScheduledAt && DateTime.UtcNow <= sessionEndTime;

        var jitsiServer = _configuration["Jitsi:ServerUrl"] ?? "https://meet.jit.si";
        var videoCallUrl = isActive ? $"{jitsiServer}/{Uri.EscapeDataString(session.RoomName)}" : null;

        return new StudySessionDto
        {
            Id = session.Id,
            Title = session.Title,
            Description = session.Description,
            ScheduledAt = session.ScheduledAt,
            DurationMinutes = session.DurationMinutes,
            MeetingLink = session.MeetingLink,
            RoomName = session.RoomName,
            VideoCallUrl = videoCallUrl,
            UseBuiltInVideoCall = session.UseBuiltInVideoCall,
            CreatedBy = new UserDto
            {
                Id = session.CreatedBy.Id,
                Email = session.CreatedBy.Email,
                Name = session.CreatedBy.Name,
                AvatarUrl = session.CreatedBy.AvatarUrl,
                CreatedAt = session.CreatedBy.CreatedAt
            },
            CreatedAt = session.CreatedAt,
            IsUpcoming = isUpcoming,
            IsActive = isActive
        };
    }

    private static string GenerateRoomName(Guid groupId)
    {
        // Create a unique, URL-safe room name
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomPart = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"studygroup-{groupId:N}-{timestamp}-{randomPart}".ToLower();
    }
}
