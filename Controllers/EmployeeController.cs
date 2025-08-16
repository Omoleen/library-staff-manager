using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeShiftService _employeeShiftService;
        private readonly IShiftService _shiftService;
        private readonly IFileService _fileService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, IEmployeeShiftService employeeShiftService, 
            IShiftService shiftService, IFileService fileService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _employeeShiftService = employeeShiftService;
            _shiftService = shiftService;
            _fileService = fileService;
            _logger = logger;
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
        public async Task<IActionResult> Create(CreateEmployeeDto employeeDto, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    employeeDto.ImagePath = await _fileService.UploadFileAsync(imageFile, "employees");
                }

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
        public async Task<IActionResult> Edit(int id, UpdateEmployeeDto employeeDto, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current employee to preserve the image path if no new image is uploaded
                    var currentEmployee = await _employeeService.GetEmployeeForUpdate(id);
                    if (currentEmployee == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete the old image if it exists
                        if (!string.IsNullOrEmpty(currentEmployee.ImagePath))
                        {
                            await _fileService.DeleteFileAsync(currentEmployee.ImagePath);
                        }

                        // Upload the new image
                        employeeDto.ImagePath = await _fileService.UploadFileAsync(imageFile, "employees");
                        _logger.LogInformation("New image uploaded for employee {EmployeeId}: {ImagePath}", id, employeeDto.ImagePath);
                    }
                    else
                    {
                        // Preserve the existing image path if no new image is uploaded
                        employeeDto.ImagePath = currentEmployee.ImagePath;
                        _logger.LogInformation("Preserving existing image for employee {EmployeeId}: {ImagePath}", id, employeeDto.ImagePath);
                    }

                    var result = await _employeeService.UpdateEmployee(id, employeeDto);
                    if (!result)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the employee. Please try again.");
                }
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
            var employee = await _employeeService.GetEmployee(id);
            if (employee != null && !string.IsNullOrEmpty(employee.ImagePath))
            {
                await _fileService.DeleteFileAsync(employee.ImagePath);
            }

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