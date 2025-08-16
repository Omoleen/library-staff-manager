using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// Controller responsible for managing library members through the web interface.
/// Provides functionality for CRUD operations on library members, including profile image management.
/// This controller requires Admin role authorization for all actions.
/// </summary>
[Authorize(Roles = "Admin")]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IFileService _fileService;
    private readonly ILogger<MembersController> _logger;

    /// <summary>
    /// Initializes a new instance of the MembersController with required services.
    /// </summary>
    /// <param name="memberService">Service for managing member data</param>
    /// <param name="borrowedBookService">Service for managing book borrowing records</param>
    /// <param name="fileService">Service for handling file operations</param>
    /// <param name="logger">Logger for the MembersController</param>
    public MembersController(IMemberService memberService, IBorrowedBookService borrowedBookService, 
        IFileService fileService, ILogger<MembersController> logger)
    {
        _memberService = memberService;
        _borrowedBookService = borrowedBookService;
        _fileService = fileService;
        _logger = logger;
    }

    /// <summary>
    /// Displays a list of all library members.
    /// </summary>
    /// <returns>A view containing a list of all members</returns>
    public async Task<IActionResult> Index()
    {
        var members = await _memberService.GetAllMembersAsync();
        return View(members);
    }

    /// <summary>
    /// Displays detailed information about a specific library member.
    /// </summary>
    /// <param name="id">The ID of the member to display</param>
    /// <returns>A view containing detailed member information</returns>
    public async Task<IActionResult> Details(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    /// <summary>
    /// Displays the form for creating a new library member.
    /// </summary>
    /// <returns>A view containing the member creation form</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Processes the creation of a new library member, including handling of profile image upload.
    /// </summary>
    /// <param name="member">The member model containing the new member's information</param>
    /// <param name="imageFile">Optional profile image file for the member</param>
    /// <returns>Redirects to the Index action if successful, otherwise returns to the creation form with validation errors</returns>
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

    /// <summary>
    /// Displays the form for editing an existing library member's information.
    /// </summary>
    /// <param name="id">The ID of the member to edit</param>
    /// <returns>A view containing the member edit form</returns>
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    /// <summary>
    /// Processes the update of an existing library member's information.
    /// </summary>
    /// <param name="id">The ID of the member to update</param>
    /// <param name="member">The member model containing the updated information</param>
    /// <param name="imageFile">Optional new profile image file for the member</param>
    /// <returns>Redirects to the Index action if successful, otherwise returns to the edit form with validation errors</returns>
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

    /// <summary>
    /// Displays the confirmation page for deleting a library member.
    /// </summary>
    /// <param name="id">The ID of the member to delete</param>
    /// <returns>A view containing the member deletion confirmation</returns>
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    /// <summary>
    /// Processes the deletion of a library member.
    /// </summary>
    /// <param name="id">The ID of the member to delete</param>
    /// <returns>Redirects to the Index action after successful deletion</returns>
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