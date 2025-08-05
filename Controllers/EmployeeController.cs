using Microsoft.AspNetCore.Mvc;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeShiftService _employeeShiftService;
        private readonly IShiftService _shiftService;

        public EmployeeController(IEmployeeService employeeService, IEmployeeShiftService employeeShiftService, IShiftService shiftService)
        {
            _employeeService = employeeService;
            _employeeShiftService = employeeShiftService;
            _shiftService = shiftService;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployees();
            return View(employees);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }

            var allShifts = await _shiftService.GetShifts();
            var assignedShifts = await _employeeShiftService.GetShiftsForEmployee(id);
            var assignedShiftIds = new HashSet<int>(assignedShifts.Select(s => s.ShiftID));
            
            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = employee,
                AssignedShifts = assignedShifts,
                AvailableShifts = allShifts.Where(s => !assignedShiftIds.Contains(s.ShiftID))
            };
            
            return View(viewModel);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.CreateEmployee(employeeDto);
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeForUpdate(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _employeeService.UpdateEmployee(id, employeeDto);
                if(!result)
                {
                    // This could be because of a concurrency issue or the employee was deleted.
                    // For now, returning NotFound is a reasonable approach.
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployee(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Employee/AssignShift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignShift(int employeeId, int shiftId)
        {
            await _employeeShiftService.LinkEmployeeToShift(new LinkEmployeeShiftDto { EmployeeID = employeeId, ShiftID = shiftId });
            return RedirectToAction(nameof(Details), new { id = employeeId });
        }

        // POST: Employee/UnassignShift
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignShift(int employeeId, int shiftId)
        {
            await _employeeShiftService.UnlinkEmployeeFromShift(employeeId, shiftId);
            return RedirectToAction(nameof(Details), new { id = employeeId });
        }
    }
} 