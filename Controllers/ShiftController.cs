using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    /// <summary>
    /// Controller responsible for managing work shifts through the web interface.
    /// Provides functionality for CRUD operations on shifts and managing employee assignments.
    /// This controller requires Admin role authorization for all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeShiftService _employeeShiftService;

        /// <summary>
        /// Initializes a new instance of the ShiftController with required services.
        /// </summary>
        /// <param name="shiftService">Service for managing shift data</param>
        /// <param name="employeeService">Service for managing employee data</param>
        /// <param name="employeeShiftService">Service for managing employee-shift relationships</param>
        public ShiftController(IShiftService shiftService, IEmployeeService employeeService, IEmployeeShiftService employeeShiftService)
        {
            _shiftService = shiftService;
            _employeeService = employeeService;
            _employeeShiftService = employeeShiftService;
        }

        /// <summary>
        /// Displays a list of all shifts in the system.
        /// </summary>
        /// <returns>A view containing a list of all shifts</returns>
        public async Task<IActionResult> Index()
        {
            var shifts = await _shiftService.GetShifts();
            return View(shifts);
        }

        /// <summary>
        /// Displays detailed information about a specific shift, including assigned and available employees.
        /// </summary>
        /// <param name="id">The ID of the shift to display</param>
        /// <returns>A view containing detailed shift information and employee assignments</returns>
        public async Task<IActionResult> Details(int id)
        {
            var shift = await _shiftService.GetShift(id);
            if (shift == null)
            {
                return NotFound();
            }

            var allEmployees = await _employeeService.GetEmployees();
            var assignedEmployees = await _employeeShiftService.GetEmployeesInShift(id);
            var assignedEmployeeIds = new HashSet<int>(assignedEmployees.Select(e => e.EmployeeID));
            
            var viewModel = new ShiftDetailsViewModel
            {
                Shift = shift,
                AssignedEmployees = assignedEmployees,
                AvailableEmployees = allEmployees.Where(e => !assignedEmployeeIds.Contains(e.EmployeeID))
            };

            return View(viewModel);
        }
        
        /// <summary>
        /// Assigns an employee to a shift.
        /// </summary>
        /// <param name="shiftId">The ID of the shift</param>
        /// <param name="employeeId">The ID of the employee to assign</param>
        /// <returns>Redirects to the shift's Details page after successful assignment</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignEmployee(int shiftId, int employeeId)
        {
            await _employeeShiftService.LinkEmployeeToShift(new LinkEmployeeShiftDto { ShiftID = shiftId, EmployeeID = employeeId });
            return RedirectToAction(nameof(Details), new { id = shiftId });
        }
        
        // POST: Shift/UnassignEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignEmployee(int shiftId, int employeeId)
        {
            await _employeeShiftService.UnlinkEmployeeFromShift(employeeId, shiftId);
            return RedirectToAction(nameof(Details), new { id = shiftId });
        }

        // GET: Shift/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shift/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShiftDto shiftDto)
        {
            if (ModelState.IsValid)
            {
                await _shiftService.CreateShift(shiftDto);
                return RedirectToAction(nameof(Index));
            }
            return View(shiftDto);
        }

        // GET: Shift/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var shift = await _shiftService.GetShift(id);
            if (shift == null)
            {
                return NotFound();
            }
            
            var updateDto = new UpdateShiftDto
            {
                StartDateTime = shift.StartDateTime,
                EndDateTime = shift.EndDateTime
            };
            
            return View(updateDto);
        }

        // POST: Shift/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateShiftDto shiftDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _shiftService.UpdateShift(id, shiftDto);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shiftDto);
        }

        // GET: Shift/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _shiftService.GetShift(id);
            if (shift == null)
            {
                return NotFound();
            }
            return View(shift);
        }

        // POST: Shift/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _shiftService.DeleteShift(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 