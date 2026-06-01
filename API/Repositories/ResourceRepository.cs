using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;

namespace StudyConnect.API.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly StudyConnectDbContext _context;

    public ResourceRepository(StudyConnectDbContext context)
    {
        _context = context;
    }

    public async Task<SharedResource?> GetByIdAsync(Guid id)
    {
        return await _context.SharedResources
            .Include(r => r.UploadedBy)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<SharedResource>> GetByGroupAsync(Guid groupId)
    {
        return await _context.SharedResources
            .Include(r => r.UploadedBy)
            .Where(r => r.GroupId == groupId)
            .OrderByDescending(r => r.UploadedAt)
            .ToListAsync();
    }

    public async Task<SharedResource> CreateAsync(SharedResource resource)
    {
        _context.SharedResources.Add(resource);
        await _context.SaveChangesAsync();
        return resource;
    }

    public async Task UpdateAsync(SharedResource resource)
    {
        _context.SharedResources.Update(resource);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var resource = await _context.SharedResources.FindAsync(id);
        if (resource != null)
        {
            _context.SharedResources.Remove(resource);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByGroupAsync(Guid groupId)
    {
        return await _context.SharedResources.CountAsync(r => r.GroupId == groupId);
    }
}
