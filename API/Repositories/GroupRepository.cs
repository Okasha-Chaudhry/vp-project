using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;

namespace StudyConnect.API.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly StudyConnectDbContext _context;

    public GroupRepository(StudyConnectDbContext context)
    {
        _context = context;
    }

    public async Task<StudyGroup?> GetByIdAsync(Guid id)
    {
        return await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .Include(g => g.Resources)
                .ThenInclude(r => r.UploadedBy)
            .Include(g => g.Sessions)
                .ThenInclude(s => s.CreatedBy)
            .Include(g => g.Tasks)
                .ThenInclude(t => t.AssignedTo)
            .Include(g => g.Tasks)
                .ThenInclude(t => t.CreatedBy)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<StudyGroup?> GetByInviteCodeAsync(string inviteCode)
    {
        return await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .FirstOrDefaultAsync(g => g.InviteCode == inviteCode.ToUpper());
    }

    public async Task<IEnumerable<StudyGroup>> GetAllAsync()
    {
        return await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudyGroup>> GetByUserAsync(Guid userId)
    {
        return await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
            .Where(g => g.CreatedByUserId == userId || g.Members.Any(m => m.UserId == userId))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudyGroup> CreateAsync(StudyGroup group)
    {
        _context.StudyGroups.Add(group);
        await _context.SaveChangesAsync();

        // Reload with navigation properties so CreatedBy is not null
        return await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .FirstAsync(g => g.Id == group.Id);
    }

    public async Task UpdateAsync(StudyGroup group)
    {
        _context.StudyGroups.Update(group);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var group = await _context.StudyGroups.FindAsync(id);
        if (group != null)
        {
            _context.StudyGroups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsMemberAsync(Guid groupId, Guid userId)
    {
        return await _context.GroupMembers
            .AnyAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
    }

    public async Task<GroupMember> AddMemberAsync(Guid groupId, Guid userId)
    {
        var member = new GroupMember
        {
            GroupId = groupId,
            UserId = userId
        };
        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid userId)
    {
        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        if (member != null)
        {
            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetMemberCountAsync(Guid groupId)
    {
        return await _context.GroupMembers.CountAsync(gm => gm.GroupId == groupId);
    }
}