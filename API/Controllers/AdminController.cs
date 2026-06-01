using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;

namespace StudyConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly StudyConnectDbContext _context;

    public AdminController(StudyConnectDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalGroups = await _context.StudyGroups.CountAsync();
        var totalResources = await _context.SharedResources.CountAsync();
        var totalSessions = await _context.StudySessions.CountAsync();
        var totalTasks = await _context.GroupTasks.CountAsync();

        return Ok(new
        {
            totalUsers,
            totalGroups,
            totalResources,
            totalSessions,
            totalTasks
        });
    }

    [HttpGet("users")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.AvatarUrl,
                u.CreatedAt,
                GroupCount = u.GroupMemberships.Count
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("groups")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllGroups()
    {
        var groups = await _context.StudyGroups
            .Include(g => g.CreatedBy)
            .Include(g => g.Members)
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Description,
                g.InviteCode,
                g.CreatedAt,
                CreatedBy = new { g.CreatedBy.Id, g.CreatedBy.Name, g.CreatedBy.Email },
                MemberCount = g.Members.Count,
                ResourceCount = g.Resources.Count,
                SessionCount = g.Sessions.Count,
                TaskCount = g.Tasks.Count
            })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("activities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentActivities()
    {
        var recentResources = await _context.SharedResources
            .OrderByDescending(r => r.UploadedAt)
            .Take(10)
            .Select(r => new { Type = "Resource", r.Title, Date = r.UploadedAt, User = r.UploadedBy.Name })
            .ToListAsync();

        var recentSessions = await _context.StudySessions
            .OrderByDescending(s => s.CreatedAt)
            .Take(10)
            .Select(s => new { Type = "Session", s.Title, Date = s.CreatedAt, User = s.CreatedBy.Name })
            .ToListAsync();

        var recentTasks = await _context.GroupTasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .Select(t => new { Type = "Task", t.Title, Date = t.CreatedAt, User = t.CreatedBy.Name })
            .ToListAsync();

        var activities = recentResources
            .Concat(recentSessions)
            .Concat(recentTasks)
            .OrderByDescending(a => a.Date)
            .Take(20);

        return Ok(activities);
    }
}
