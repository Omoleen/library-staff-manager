using Microsoft.AspNetCore.Mvc;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeShiftService _employeeShiftService;

        public ShiftController(IShiftService shiftService, IEmployeeService employeeService, IEmployeeShiftService employeeShiftService)
        {
            _shiftService = shiftService;
            _employeeService = employeeService;
            _employeeShiftService = employeeShiftService;
        }

        // GET: Shift
        public async Task<IActionResult> Index()
        {
            var shifts = await _shiftService.GetShifts();
            return View(shifts);
        }

        // GET: Shift/Details/5
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
        
        // POST: Shift/AssignEmployee
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