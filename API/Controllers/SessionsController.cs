using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyConnect.API.Dtos;
using StudyConnect.API.Services.Interfaces;

namespace StudyConnect.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim == null ? Guid.Empty : Guid.Parse(claim.Value);
    }

    [HttpGet("sessions/{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var session = await _sessionService.GetSessionForUserAsync(id, userId);
            if (session == null) return NotFound();
            return Ok(session);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("groups/{groupId:guid}/sessions")]
    public async Task<IActionResult> GetGroupSessions(Guid groupId)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var sessions = await _sessionService.GetGroupSessionsAsync(groupId, userId);
            return Ok(sessions);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("groups/{groupId:guid}/sessions/{id:guid}")]
    public async Task<IActionResult> GetSession(Guid groupId, Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var session = await _sessionService.GetSessionForUserAsync(id, userId);
            if (session == null) return NotFound();
            return Ok(session);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("groups/{groupId:guid}/sessions")]
    public async Task<IActionResult> CreateSession(Guid groupId, [FromBody] CreateSessionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var session = await _sessionService.CreateSessionAsync(userId, groupId, request);
            return CreatedAtAction(nameof(GetSession), new { groupId, id = session.Id }, session);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("groups/{groupId:guid}/sessions/{id:guid}")]
    public async Task<IActionResult> UpdateSession(Guid groupId, Guid id, [FromBody] UpdateSessionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var session = await _sessionService.UpdateSessionAsync(id, userId, request);
            if (session == null) return NotFound();
            return Ok(session);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("groups/{groupId:guid}/sessions/{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid groupId, Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _sessionService.DeleteSessionAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
