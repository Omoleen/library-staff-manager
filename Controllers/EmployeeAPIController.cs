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
    /// API controller for managing employee data through RESTful endpoints.
    /// Provides CRUD operations for employee management through API calls.
    /// This controller requires Admin role authorization for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the EmployeeAPIController.
        /// </summary>
        /// <param name="employeeService">The service for managing employee data</param>
        public EmployeeAPIController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves all employees in the system.
        /// </summary>
        /// <returns>A list of all employees with their details</returns>
        /// <response code="200">Returns the list of employees</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employeeDtos = await _employeeService.GetEmployees();
            return Ok(employeeDtos);
        }

        /// <summary>
        /// Retrieves a specific employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve</param>
        /// <returns>The employee details</returns>
        /// <response code="200">Returns the requested employee</response>
        /// <response code="404">If the employee is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeModel(int id)
        {
            var dto = await _employeeService.GetEmployee(id);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }

        /// <summary>
        /// Updates an existing employee's information.
        /// </summary>
        /// <param name="id">The ID of the employee to update</param>
        /// <param name="dto">The updated employee information</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the employee was successfully updated</response>
        /// <response code="404">If the employee is not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeModel(int id, UpdateEmployeeDto dto)
        {
            var result = await _employeeService.UpdateEmployee(id, dto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Creates a new employee in the system.
        /// </summary>
        /// <param name="dto">The employee information to create</param>
        /// <returns>The newly created employee</returns>
        /// <response code="201">Returns the newly created employee</response>
        /// <response code="400">If the employee data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployeeModel(CreateEmployeeDto dto)
        {
            var resultDto = await _employeeService.CreateEmployee(dto);
            return CreatedAtAction("GetEmployeeModel", new { id = resultDto.EmployeeID }, resultDto);
        }

        /// <summary>
        /// Deletes a specific employee from the system.
        /// </summary>
        /// <param name="id">The ID of the employee to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the employee was successfully deleted</response>
        /// <response code="404">If the employee is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeModel(int id)
        {
            var result = await _employeeService.DeleteEmployee(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
} 
