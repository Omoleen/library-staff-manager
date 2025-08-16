using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class MembersAPIController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersAPIController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    // GET: api/MembersAPI
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberModel>>> GetMembers()
    {
        var members = await _memberService.GetAllMembersAsync();
        return Ok(members);
    }

    // GET: api/MembersAPI/5
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

    // PUT: api/MembersAPI/5
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

    // POST: api/MembersAPI
    [HttpPost]
    public async Task<ActionResult<MemberModel>> PostMember(MemberModel member)
    {
        var createdMember = await _memberService.CreateMemberAsync(member);
        return CreatedAtAction("GetMember", new { id = createdMember.MemberId }, createdMember);
    }

    // DELETE: api/MembersAPI/5
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