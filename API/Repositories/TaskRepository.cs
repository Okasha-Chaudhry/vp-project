using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;
using TaskStatus = StudyConnect.API.Models.TaskStatus;
namespace StudyConnect.API.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly StudyConnectDbContext _context;

    public TaskRepository(StudyConnectDbContext context)
    {
        _context = context;
    }

    public async Task<GroupTask?> GetByIdAsync(Guid id)
    {
        return await _context.GroupTasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<GroupTask>> GetByGroupAsync(Guid groupId)
    {
        return await _context.GroupTasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.GroupId == groupId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupTask>> GetByUserAsync(Guid userId)
    {
        return await _context.GroupTasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.AssignedToUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<GroupTask> CreateAsync(GroupTask task)
    {
        _context.GroupTasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(GroupTask task)
    {
        if (task.Status == TaskStatus.Completed && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (task.Status != TaskStatus.Completed)
        {
            task.CompletedAt = null;
        }

        _context.GroupTasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _context.GroupTasks.FindAsync(id);
        if (task != null)
        {
            _context.GroupTasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByGroupAsync(Guid groupId)
    {
        return await _context.GroupTasks.CountAsync(t => t.GroupId == groupId);
    }
}
