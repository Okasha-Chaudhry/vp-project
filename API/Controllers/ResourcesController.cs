using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyConnect.API.Dtos;
using StudyConnect.API.Services.Interfaces;

namespace StudyConnect.API.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/[controller]")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim == null ? Guid.Empty : Guid.Parse(claim.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroupResources(Guid groupId)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var resources = await _resourceService.GetGroupResourcesAsync(groupId, userId);
            return Ok(resources);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetResource(Guid groupId, Guid id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        if (resource == null) return NotFound();
        return Ok(resource);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResource(Guid groupId, [FromBody] CreateResourceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var resource = await _resourceService.CreateResourceAsync(userId, groupId, request);
            return CreatedAtAction(nameof(GetResource), new { groupId, id = resource.Id }, resource);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateResource(Guid groupId, Guid id, [FromBody] UpdateResourceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var resource = await _resourceService.UpdateResourceAsync(id, userId, request);
            if (resource == null) return NotFound();
            return Ok(resource);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteResource(Guid groupId, Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _resourceService.DeleteResourceAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
