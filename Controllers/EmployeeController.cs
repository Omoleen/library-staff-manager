using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    /// <summary>
    /// Controller responsible for managing employee-related operations in the staff management system.
    /// Provides functionality for CRUD operations on employees and managing their shift assignments.
    /// This controller requires Admin role authorization for all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeShiftService _employeeShiftService;
        private readonly IShiftService _shiftService;
        private readonly IFileService _fileService;
        private readonly ILogger<EmployeeController> _logger;

        /// <summary>
        /// Initializes a new instance of the EmployeeController with required services.
        /// </summary>
        /// <param name="employeeService">Service for managing employee data</param>
        /// <param name="employeeShiftService">Service for managing employee-shift relationships</param>
        /// <param name="shiftService">Service for managing shift data</param>
        /// <param name="fileService">Service for handling file operations</param>
        /// <param name="logger">Logger for the EmployeeController</param>
        public EmployeeController(IEmployeeService employeeService, IEmployeeShiftService employeeShiftService, 
            IShiftService shiftService, IFileService fileService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _employeeShiftService = employeeShiftService;
            _shiftService = shiftService;
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of all employees in the system.
        /// </summary>
        /// <returns>A view containing a list of all employees</returns>
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployees();
            return View(employees);
        }

        /// <summary>
        /// Displays detailed information about a specific employee, including their assigned and available shifts.
        /// </summary>
        /// <param name="id">The ID of the employee to display</param>
        /// <returns>A view containing detailed employee information and shift assignments</returns>
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

        /// <summary>
        /// Displays the form for creating a new employee.
        /// </summary>
        /// <returns>A view containing the employee creation form</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the creation of a new employee, including handling of profile image upload.
        /// </summary>
        /// <param name="employeeDto">The employee data transfer object containing the new employee's information</param>
        /// <param name="imageFile">Optional profile image file for the employee</param>
        /// <returns>Redirects to the Index action if successful, otherwise returns to the creation form with validation errors</returns>
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

        /// <summary>
        /// Displays the form for editing an existing employee's information.
        /// </summary>
        /// <param name="id">The ID of the employee to edit</param>
        /// <returns>A view containing the employee edit form</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeForUpdate(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        /// <summary>
        /// Processes the update of an existing employee's information, including handling of profile image updates.
        /// </summary>
        /// <param name="id">The ID of the employee to update</param>
        /// <param name="employeeDto">The employee data transfer object containing the updated information</param>
        /// <param name="imageFile">Optional new profile image file for the employee</param>
        /// <returns>Redirects to the Index action if successful, otherwise returns to the edit form with validation errors</returns>
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

        /// <summary>
        /// Displays the confirmation page for deleting an employee.
        /// </summary>
        /// <param name="id">The ID of the employee to delete</param>
        /// <returns>A view containing the employee deletion confirmation</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        /// <summary>
        /// Processes the deletion of an employee, including removal of their profile image.
        /// </summary>
        /// <param name="id">The ID of the employee to delete</param>
        /// <returns>Redirects to the Index action after successful deletion</returns>
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

        /// <summary>
        /// Assigns a shift to an employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <param name="shiftId">The ID of the shift to assign</param>
        /// <returns>Redirects to the employee's Details page after successful assignment</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignShift(int employeeId, int shiftId)
        {
            await _employeeShiftService.LinkEmployeeToShift(new LinkEmployeeShiftDto { EmployeeID = employeeId, ShiftID = shiftId });
            return RedirectToAction(nameof(Details), new { id = employeeId });
        }

        /// <summary>
        /// Removes a shift assignment from an employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee</param>
        /// <param name="shiftId">The ID of the shift to unassign</param>
        /// <returns>Redirects to the employee's Details page after successful unassignment</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnassignShift(int employeeId, int shiftId)
        {
            await _employeeShiftService.UnlinkEmployeeFromShift(employeeId, shiftId);
            return RedirectToAction(nameof(Details), new { id = employeeId });
        }
    }
} 