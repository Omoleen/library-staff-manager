using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers;

/// <summary>
/// Controller responsible for managing book borrowing records through the web interface.
/// Provides functionality for tracking book loans, returns, and managing borrowing history.
/// This controller requires Admin role authorization for all actions.
/// </summary>
[Authorize(Roles = "Admin")]
public class BorrowedBooksController : Controller
{
    private readonly IBorrowedBookService _borrowedBookService;
    private readonly IBookService _bookService;
    private readonly IMemberService _memberService;
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initializes a new instance of the BorrowedBooksController with required services.
    /// </summary>
    /// <param name="borrowedBookService">Service for managing borrowed book records</param>
    /// <param name="bookService">Service for managing book data</param>
    /// <param name="memberService">Service for managing member data</param>
    /// <param name="shiftService">Service for managing shift data</param>
    /// <param name="employeeService">Service for managing employee data</param>
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

    /// <summary>
    /// Displays a list of all borrowed books in the system.
    /// </summary>
    /// <returns>A view containing a list of all borrowed books</returns>
    public async Task<IActionResult> Index()
    {
        var borrowedBooks = await _borrowedBookService.GetAllBorrowedBooksAsync();
        return View(borrowedBooks);
    }

    /// <summary>
    /// Displays detailed information about a specific borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to display</param>
    /// <returns>A view containing detailed borrowed book information</returns>
    public async Task<IActionResult> Details(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }

        return View(borrowedBook);
    }

    /// <summary>
    /// Displays the form for creating a new borrowed book record.
    /// Optionally pre-fills the form with member and book information.
    /// Also provides information about current shift and available employees.
    /// </summary>
    /// <param name="memberId">Optional ID of the member who is borrowing the book</param>
    /// <param name="bookId">Optional ID of the book being borrowed</param>
    /// <returns>A view containing the borrowed book creation form</returns>
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

    /// <summary>
    /// Processes the creation of a new borrowed book record.
    /// Automatically assigns the current shift if none is specified.
    /// </summary>
    /// <param name="borrowedBook">The borrowed book record to create</param>
    /// <returns>Redirects to the appropriate view based on context (member details, book details, or index)</returns>
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

    /// <summary>
    /// Displays the form for editing an existing borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to edit</param>
    /// <returns>A view containing the borrowed book edit form</returns>
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

    /// <summary>
    /// Processes the update of an existing borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to update</param>
    /// <param name="borrowedBook">The updated borrowed book information</param>
    /// <returns>Redirects to the Index action if successful, otherwise returns to the edit form with validation errors</returns>
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

    /// <summary>
    /// Displays the confirmation page for deleting a borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to delete</param>
    /// <returns>A view containing the borrowed book deletion confirmation</returns>
    public async Task<IActionResult> Delete(int id)
    {
        var borrowedBook = await _borrowedBookService.GetBorrowedBookByIdAsync(id);
        if (borrowedBook == null)
        {
            return NotFound();
        }
        return View(borrowedBook);
    }

    /// <summary>
    /// Processes the deletion of a borrowed book record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to delete</param>
    /// <returns>Redirects to the appropriate view based on context (member details or index)</returns>
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

    /// <summary>
    /// Displays the form for recording a book return.
    /// Provides information about current shift and available employees.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to process return for</param>
    /// <returns>A view containing the book return form</returns>
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

    /// <summary>
    /// Processes the return of a borrowed book.
    /// Automatically assigns the current shift and receiving employee to the return record.
    /// </summary>
    /// <param name="id">The ID of the borrowed book record to process return for</param>
    /// <param name="receivedByEmployeeId">The ID of the employee receiving the returned book</param>
    /// <returns>Redirects to the appropriate view based on context (member details or index)</returns>
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