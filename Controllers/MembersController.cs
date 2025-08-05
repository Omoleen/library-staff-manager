using Microsoft.AspNetCore.Mvc;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly IBorrowedBookService _borrowedBookService;

    public MembersController(IMemberService memberService, IBorrowedBookService borrowedBookService)
    {
        _memberService = memberService;
        _borrowedBookService = borrowedBookService;
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
    public async Task<IActionResult> Create(MemberModel member)
    {
        if (ModelState.IsValid)
        {
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
    public async Task<IActionResult> Edit(int id, MemberModel member)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _memberService.UpdateMemberAsync(id, member);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
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
            await _memberService.DeleteMemberAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 