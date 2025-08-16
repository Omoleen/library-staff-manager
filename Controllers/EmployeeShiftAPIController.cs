using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    /// <summary>
    /// API controller for managing employee-shift assignments through RESTful endpoints.
    /// Provides operations for managing relationships between employees and their work shifts.
    /// This controller requires Admin role authorization for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmployeeShiftAPIController : ControllerBase
    {
        private readonly IEmployeeShiftService _employeeShiftService;

        /// <summary>
        /// Initializes a new instance of the EmployeeShiftAPIController.
        /// </summary>
        /// <param name="employeeShiftService">The service for managing employee-shift relationships</param>
        public EmployeeShiftAPIController(IEmployeeShiftService employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }

        /// <summary>
        /// Retrieves all employee-shift assignments in the system.
        /// </summary>
        /// <returns>A list of all employee-shift assignments</returns>
        /// <response code="200">Returns the list of employee-shift assignments</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeShiftModel>>> GetEmployeeShiftModel()
        {
            return Ok(await _employeeShiftService.GetAllEmployeeShiftsAsync());
        }

        /// <summary>
        /// Retrieves a specific employee-shift assignment by its ID.
        /// </summary>
        /// <param name="id">The ID of the employee-shift assignment to retrieve</param>
        /// <returns>The employee-shift assignment details</returns>
        /// <response code="200">Returns the requested employee-shift assignment</response>
        /// <response code="404">If the employee-shift assignment is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeShiftModel>> GetEmployeeShiftModel(int id)
        {
            var employeeShiftModel = await _employeeShiftService.GetEmployeeShift(id);

            if (employeeShiftModel == null)
            {
                return NotFound();
            }

            return employeeShiftModel;
        }

        /// <summary>
        /// Updates an existing employee-shift assignment.
        /// </summary>
        /// <param name="id">The ID of the employee-shift assignment to update</param>
        /// <param name="employeeShiftModel">The updated employee-shift assignment information</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the employee-shift assignment was successfully updated</response>
        /// <response code="400">If the employee-shift data is invalid</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeShiftModel(int id, EmployeeShiftModel employeeShiftModel)
        {
            var result = await _employeeShiftService.UpdateEmployeeShift(id, employeeShiftModel);
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new employee-shift assignment.
        /// </summary>
        /// <param name="employeeShiftModel">The employee-shift assignment information to create</param>
        /// <returns>The newly created employee-shift assignment</returns>
        /// <response code="201">Returns the newly created employee-shift assignment</response>
        /// <response code="400">If the employee-shift data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<EmployeeShiftModel>> PostEmployeeShiftModel(EmployeeShiftModel employeeShiftModel)
        {
            var newModel = await _employeeShiftService.CreateEmployeeShift(employeeShiftModel);
            return CreatedAtAction("GetEmployeeShiftModel", new { id = newModel.EmployeeShiftID }, newModel);
        }

        /// <summary>
        /// Deletes a specific employee-shift assignment.
        /// </summary>
        /// <param name="id">The ID of the employee-shift assignment to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the employee-shift assignment was successfully deleted</response>
        /// <response code="404">If the employee-shift assignment is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeShiftModel(int id)
        {
            var result = await _employeeShiftService.DeleteEmployeeShift(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Links an employee to a shift.
        /// </summary>
        /// <param name="dto">The data transfer object containing employee and shift IDs to link</param>
        /// <returns>The created link information if successful</returns>
        /// <response code="200">If the employee was successfully linked to the shift</response>
        /// <response code="400">If the employee or shift IDs are invalid</response>
        /// <response code="409">If the employee is already linked to the shift</response>
        [HttpPost("link")]
        public async Task<IActionResult> LinkEmployeeToShift([FromBody] LinkEmployeeShiftDto dto)
        {
            if (dto == null || dto.EmployeeID == 0 || dto.ShiftID == 0)
                return BadRequest("Invalid EmployeeID or ShiftID");

            var result = await _employeeShiftService.LinkEmployeeToShift(dto);
            if (result == null)
            {
                return Conflict("Employee is already linked to this shift.");
            }
            return Ok(dto);
        }

        /// <summary>
        /// Removes the link between an employee and a shift.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to unlink</param>
        /// <param name="shiftId">The ID of the shift to unlink</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the employee was successfully unlinked from the shift</response>
        /// <response code="404">If the link between employee and shift is not found</response>
        [HttpDelete("unlink")]
        public async Task<IActionResult> UnlinkEmployeeFromShift([FromQuery] int employeeId, [FromQuery] int shiftId)
        {
            var result = await _employeeShiftService.UnlinkEmployeeFromShift(employeeId, shiftId);
            if (!result)
            {
                return NotFound("Link not found.");
            }
            return NoContent();
        }

        /// <summary>
        /// Retrieves all employees assigned to a specific shift.
        /// </summary>
        /// <param name="shiftId">The ID of the shift to get employees for</param>
        /// <returns>A list of employees assigned to the specified shift</returns>
        /// <response code="200">Returns the list of employees in the shift</response>
        [HttpGet("employees-in-shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesInShift(int shiftId)
        {
            var employeeDtos = await _employeeShiftService.GetEmployeesInShift(shiftId);
            return Ok(employeeDtos);
        }

        /// <summary>
        /// Retrieves all shifts assigned to a specific employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to get shifts for</param>
        /// <returns>A list of shifts assigned to the specified employee</returns>
        /// <response code="200">Returns the list of shifts for the employee</response>
        [HttpGet("shifts-for-employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShiftsForEmployee(int employeeId)
        {
            var shiftDtos = await _employeeShiftService.GetShiftsForEmployee(employeeId);
            return Ok(shiftDtos);
        }
    }
}
