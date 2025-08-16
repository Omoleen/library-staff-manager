using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

[Authorize(Roles = "Admin")]
public class BorrowedBooksController : Controller
{
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;

    public BorrowedBooksController(
        IBorrowedBookService borrowedBookService, 
        IBookService bookService, 
        IMemberService memberService,
        IShiftService shiftService,
        IEmployeeService employeeService)
    {
        _borrowedBookService = borrowedBookService;
        _bookService = bookService;
        _memberService = memberService;
        _shiftService = shiftService;
        _employeeService = employeeService;
    }

    // GET: BorrowedBooks
    public async Task<IActionResult> Index()
    {
        var borrowedBooks = await _borrowedBookService.GetAllBorrowedBooksAsync();
        return View(borrowedBooks);
    }

    // GET: BorrowedBooks/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }

        return View(borrowedBook);
    }

    // GET: BorrowedBooks/Create
    public async Task<IActionResult> Create(int? memberId = null, int? bookId = null)
    {
        ViewBag.Books = await _bookService.GetAllBooksAsync();
        ViewBag.Members = await _memberService.GetAllMembersAsync();
        ViewBag.Employees = await _employeeService.GetEmployees();
        
        // Get current active shift
        var currentShifts = await _shiftService.GetShifts();
        var currentShift = currentShifts.FirstOrDefault(s => 
            s.StartDateTime <= DateTime.Now && s.EndDateTime >= DateTime.Now);
        ViewBag.CurrentShift = currentShift;

        // Get employees working in current shift
        if (currentShift != null)
        {
            var employeeShifts = await _employeeService.GetEmployeesForShift(currentShift.ShiftID);
            ViewBag.CurrentShiftEmployees = employeeShifts;
        }
        
        var model = new BorrowedBookModel
        {
            BorrowDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),
            BorrowedDuringShiftId = currentShift?.ShiftID
        };

        if (memberId.HasValue)
        {
            var member = await _memberService.GetMemberByIdAsync(memberId.Value);
            if (member != null)
            {
                model.MemberId = member.MemberId;
                model.Member = member;
            }
        }

        if (bookId.HasValue)
        {
            var book = await _bookService.GetBookByIdAsync(bookId.Value);
            if (book != null)
            {
                model.BookId = book.BookId;
                model.Book = book;
            }
        }

        return View(model);
    }

    // POST: BorrowedBooks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BorrowedBookModel borrowedBook)
    {
        if (ModelState.IsValid)
        {
            // If no shift was specified, try to get current shift
            if (!borrowedBook.BorrowedDuringShiftId.HasValue)
            {
                var currentShifts = await _shiftService.GetShifts();
                var currentShift = currentShifts.FirstOrDefault(s => 
                    s.StartDateTime <= DateTime.Now && s.EndDateTime >= DateTime.Now);
                borrowedBook.BorrowedDuringShiftId = currentShift?.ShiftID;
            }

            await _borrowedBookService.CreateBorrowedBookAsync(borrowedBook);
            
            // If this was created from a member's details page, return there
            if (borrowedBook.MemberId > 0)
            {
                return RedirectToAction("Details", "Members", new { id = borrowedBook.MemberId });
            }
            
            // If this was created from a book's details page, return there
            if (borrowedBook.BookId > 0)
            {
                return RedirectToAction("Details", "Books", new { id = borrowedBook.BookId });
            }
            
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Books = await _bookService.GetAllBooksAsync();
        ViewBag.Members = await _memberService.GetAllMembersAsync();
        ViewBag.Employees = await _employeeService.GetEmployees();
        return View(borrowedBook);
    }

    // GET: BorrowedBooks/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }

        ViewBag.Books = await _bookService.GetAllBooksAsync();
        ViewBag.Members = await _memberService.GetAllMembersAsync();
        ViewBag.Employees = await _employeeService.GetEmployees();
        return View(borrowedBook);
    }

    // POST: BorrowedBooks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BorrowedBookModel borrowedBook)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _borrowedBookService.UpdateBorrowedBookAsync(id, borrowedBook);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        ViewBag.Books = await _bookService.GetAllBooksAsync();
        ViewBag.Members = await _memberService.GetAllMembersAsync();
        ViewBag.Employees = await _employeeService.GetEmployees();
        return View(borrowedBook);
    }

    // GET: BorrowedBooks/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }
        return View(borrowedBook);
    }

    // POST: BorrowedBooks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
            await _borrowedBookService.DeleteBorrowedBookAsync(id);
            
            // If this was deleted from a member's details page, return there
            if (borrowedBook?.MemberId > 0)
            {
                return RedirectToAction("Details", "Members", new { id = borrowedBook.MemberId });
            }
            
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET: BorrowedBooks/Return/5
    public async Task<IActionResult> Return(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }

        // Get current active shift
        var currentShifts = await _shiftService.GetShifts();
        var currentShift = currentShifts.FirstOrDefault(s => 
            s.StartDateTime <= DateTime.Now && s.EndDateTime >= DateTime.Now);
        ViewBag.CurrentShift = currentShift;

        // Get employees working in current shift
        if (currentShift != null)
        {
            var employeeShifts = await _employeeService.GetEmployeesForShift(currentShift.ShiftID);
            ViewBag.CurrentShiftEmployees = employeeShifts;
        }

        ViewBag.Employees = await _employeeService.GetEmployees();
        return View(borrowedBook);
    }

    // POST: BorrowedBooks/Return/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id, int receivedByEmployeeId)
    {
        try
        {
            // Get current shift
            var currentShifts = await _shiftService.GetShifts();
            var currentShift = currentShifts.FirstOrDefault(s => 
                s.StartDateTime <= DateTime.Now && s.EndDateTime >= DateTime.Now);

            var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
            if (borrowedBook != null)
            {
                borrowedBook.ReturnedDuringShiftId = currentShift?.ShiftID;
                borrowedBook.ReceivedByEmployeeId = receivedByEmployeeId;
                await _borrowedBookService.ReturnBookAsync(id);
            }
            
            // If this was returned from a member's details page, return there
            if (borrowedBook?.MemberId > 0)
            {
                return RedirectToAction("Details", "Members", new { id = borrowedBook.MemberId });
            }
            
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 