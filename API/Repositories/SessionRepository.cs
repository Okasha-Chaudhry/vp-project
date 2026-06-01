using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;

namespace StudyConnect.API.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly StudyConnectDbContext _context;

    public SessionRepository(StudyConnectDbContext context)
    {
        _context = context;
    }

    public async Task<StudySession?> GetByIdAsync(Guid id)
    {
        return await _context.StudySessions
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<StudySession>> GetByGroupAsync(Guid groupId)
    {
        return await _context.StudySessions
            .Include(s => s.CreatedBy)
            .Where(s => s.GroupId == groupId)
            .OrderBy(s => s.ScheduledAt)
            .ToListAsync();
    }

    public async Task<StudySession> CreateAsync(StudySession session)
    {
        _context.StudySessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(StudySession session)
    {
        _context.StudySessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var session = await _context.StudySessions.FindAsync(id);
        if (session != null)
        {
            _context.StudySessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByGroupAsync(Guid groupId)
    {
        return await _context.StudySessions.CountAsync(s => s.GroupId == groupId);
    }
}
