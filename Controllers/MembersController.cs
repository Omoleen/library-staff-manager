using Microsoft.AspNetCore.Mvc;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IFileService _fileService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMemberService memberService, IBorrowedBookService borrowedBookService, 
        IFileService fileService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _borrowedBookService = borrowedBookService;
        _fileService = fileService;
        _logger = logger;
    }

    // GET: Members
    public async Task<IActionResult> Index()
    {
        var members = await _memberService.GetAllMembersAsync();
        return View(members);
    }

    // GET: Members/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // GET: Members/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Members/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MemberModel member, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                member.ImagePath = await _fileService.UploadFileAsync(imageFile, "members");
            }

            await _memberService.CreateMemberAsync(member);
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    // GET: Members/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST: Members/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MemberModel member, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Get the current member to preserve the image path if no new image is uploaded
                var currentMember = await _memberService.GetMemberByIdAsync(id);
                if (currentMember == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(currentMember.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(currentMember.ImagePath);
                    }

                    // Upload the new image
                    member.ImagePath = await _fileService.UploadFileAsync(imageFile, "members");
                    _logger.LogInformation("New image uploaded for member {MemberId}: {ImagePath}", id, member.ImagePath);
                }
                else
                {
                    // Preserve the existing image path if no new image is uploaded
                    member.ImagePath = currentMember.ImagePath;
                    _logger.LogInformation("Preserving existing image for member {MemberId}: {ImagePath}", id, member.ImagePath);
                }

                await _memberService.UpdateMemberAsync(id, member);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating member {MemberId}", id);
                ModelState.AddModelError("", "An error occurred while updating the member. Please try again.");
            }
        }
        return View(member);
    }

    // GET: Members/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST: Members/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var member = await _memberService.GetMemberByIdAsync(id);
            if (member != null && !string.IsNullOrEmpty(member.ImagePath))
            {
                await _fileService.DeleteFileAsync(member.ImagePath);
            }

            await _memberService.DeleteMemberAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 