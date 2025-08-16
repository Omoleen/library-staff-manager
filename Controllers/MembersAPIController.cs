using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// API controller for managing library members through RESTful endpoints.
/// Provides CRUD operations for member management through API calls.
/// This controller requires Admin role authorization for all endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class MembersAPIController : ControllerBase
{
    private readonly IMemberService _memberService;

    /// <summary>
    /// Initializes a new instance of the MembersAPIController.
    /// </summary>
    /// <param name="memberService">The service for managing member data</param>
    public MembersAPIController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Retrieves all library members in the system.
    /// </summary>
    /// <returns>A list of all members with their details</returns>
    /// <response code="200">Returns the list of members</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberModel>>> GetMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    /// <summary>
    /// Retrieves a specific library member by their ID.
    /// </summary>
    /// <param name="id">The ID of the member to retrieve</param>
    /// <returns>The member details</returns>
    /// <response code="200">Returns the requested member</response>
    /// <response code="404">If the member is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<MemberModel>> GetMember(int id)
    {
        try
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            return Ok(member);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Updates an existing library member's information.
    /// </summary>
    /// <param name="id">The ID of the member to update</param>
    /// <param name="member">The updated member information</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the member was successfully updated</response>
    /// <response code="400">If the member data is invalid</response>
    /// <response code="404">If the member is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMember(int id, MemberModel member)
    {
        try
        {
            await _memberService.UpdateMemberAsync(id, member);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Creates a new library member in the system.
    /// </summary>
    /// <param name="member">The member information to create</param>
    /// <returns>The newly created member</returns>
    /// <response code="201">Returns the newly created member</response>
    /// <response code="400">If the member data is invalid</response>
    [HttpPost]
    public async Task<ActionResult<MemberModel>> PostMember(MemberModel member)
    {
        var createdMember = await _memberService.CreateMemberAsync(member);
        return CreatedAtAction("GetMember", new { id = createdMember.MemberId }, createdMember);
    }

    /// <summary>
    /// Deletes a specific library member from the system.
    /// </summary>
    /// <param name="id">The ID of the member to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the member was successfully deleted</response>
    /// <response code="404">If the member is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        try
        {
            await _memberService.DeleteMemberAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 