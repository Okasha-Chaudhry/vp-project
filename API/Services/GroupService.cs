using StudyConnect.API.Dtos;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;
using StudyConnect.API.Services.Interfaces;
using TaskStatus = StudyConnect.API.Models.TaskStatus;
namespace StudyConnect.API.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ITaskRepository _taskRepository;

    public GroupService(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IResourceRepository resourceRepository,
        ISessionRepository sessionRepository,
        ITaskRepository taskRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _resourceRepository = resourceRepository;
        _sessionRepository = sessionRepository;
        _taskRepository = taskRepository;
    }

    public async Task<StudyGroupDto> CreateGroupAsync(Guid userId, CreateGroupRequest request)
    {
        var group = new StudyGroup
        {
            Name = request.Name,
            Description = request.Description,
            CreatedByUserId = userId
        };

        var created = await _groupRepository.CreateAsync(group);
        await _groupRepository.AddMemberAsync(created.Id, userId);

        return await MapToDtoAsync(created, userId);
    }

    public async Task<StudyGroupDto?> JoinGroupAsync(Guid userId, JoinGroupRequest request)
    {
        var group = await _groupRepository.GetByInviteCodeAsync(request.InviteCode);
        if (group == null) return null;

        var isMember = await _groupRepository.IsMemberAsync(group.Id, userId);
        if (isMember) throw new InvalidOperationException("You are already a member of this group");

        await _groupRepository.AddMemberAsync(group.Id, userId);

        var updated = await _groupRepository.GetByIdAsync(group.Id);
        return updated == null ? null : await MapToDtoAsync(updated, userId);
    }

    public async Task<GroupDetailDto?> GetGroupDetailAsync(Guid groupId, Guid userId)
    {
        var isMember = await _groupRepository.IsMemberAsync(groupId, userId);
        if (!isMember) return null;

        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null) return null;

        var dto = new GroupDetailDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            InviteCode = group.InviteCode,
            CreatedBy = MapToUserDto(group.CreatedBy),
            CreatedAt = group.CreatedAt,
            MemberCount = group.Members.Count,
            ResourceCount = group.Resources.Count,
            SessionCount = group.Sessions.Count,
            TaskCount = group.Tasks.Count,
            IsMember = true,
            UserRole = group.CreatedByUserId == userId ? "Admin" : "Member",
            Members = group.Members.Select(m => new GroupMemberDto
            {
                Id = m.Id,
                User = MapToUserDto(m.User),
                Role = m.Role.ToString(),
                JoinedAt = m.JoinedAt
            }).ToList(),
            Resources = group.Resources.Select(r => new SharedResourceDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Type = r.Type.ToString(),
                Content = r.Content,
                UploadedBy = MapToUserDto(r.UploadedBy),
                UploadedAt = r.UploadedAt
            }).ToList(),
            Sessions = group.Sessions.Select(s => new StudySessionDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                ScheduledAt = s.ScheduledAt,
                DurationMinutes = s.DurationMinutes,
                MeetingLink = s.MeetingLink,
                RoomName = s.RoomName,
                VideoCallUrl = s.UseBuiltInVideoCall
                    ? $"https://meet.jit.si/{Uri.EscapeDataString(s.RoomName)}"
                    : s.MeetingLink,
                UseBuiltInVideoCall = s.UseBuiltInVideoCall,
                CreatedBy = MapToUserDto(s.CreatedBy),
                CreatedAt = s.CreatedAt,
                IsUpcoming = s.ScheduledAt > DateTime.UtcNow,
                IsActive = DateTime.UtcNow >= s.ScheduledAt.AddMinutes(-15)
                    && DateTime.UtcNow <= s.ScheduledAt.AddMinutes(s.DurationMinutes)
            }).ToList(),
            Tasks = group.Tasks.Select(t => new GroupTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                AssignedTo = t.AssignedTo == null ? null : MapToUserDto(t.AssignedTo),
                CreatedBy = MapToUserDto(t.CreatedBy),
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                IsOverdue = t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != TaskStatus.Completed
            }).ToList()
        };

        return dto;
    }

    public async Task<IEnumerable<StudyGroupDto>> GetUserGroupsAsync(Guid userId)
    {
        var groups = await _groupRepository.GetByUserAsync(userId);
        var tasks = groups.Select(g => MapToDtoAsync(g, userId));
        return await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<StudyGroupDto>> GetAllGroupsAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        var tasks = groups.Select(g => MapToDtoAsync(g, Guid.Empty));
        return await Task.WhenAll(tasks);
    }

    public async Task LeaveGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null) throw new KeyNotFoundException("Group not found");
        if (group.CreatedByUserId == userId) throw new InvalidOperationException("Group creator cannot leave");

        await _groupRepository.RemoveMemberAsync(groupId, userId);
    }

    public async Task DeleteGroupAsync(Guid groupId, Guid userId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        if (group == null) throw new KeyNotFoundException("Group not found");
        if (group.CreatedByUserId != userId) throw new UnauthorizedAccessException("Only group creator can delete");

        await _groupRepository.DeleteAsync(groupId);
    }

    private async Task<StudyGroupDto> MapToDtoAsync(StudyGroup group, Guid currentUserId)
    {
        var isMember = currentUserId != Guid.Empty && await _groupRepository.IsMemberAsync(group.Id, currentUserId);
        var member = group.Members.FirstOrDefault(m => m.UserId == currentUserId);

        return new StudyGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            InviteCode = group.InviteCode,
            CreatedBy = MapToUserDto(group.CreatedBy),
            CreatedAt = group.CreatedAt,
            MemberCount = group.Members.Count,
            ResourceCount = group.Resources?.Count ?? 0,
            SessionCount = group.Sessions?.Count ?? 0,
            TaskCount = group.Tasks?.Count ?? 0,
            IsMember = isMember,
            UserRole = group.CreatedByUserId == currentUserId ? "Admin" : member?.Role.ToString() ?? ""
        };
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };
    }
}
