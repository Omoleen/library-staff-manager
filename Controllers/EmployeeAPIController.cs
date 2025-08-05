using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAPIController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeAPIController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/EmployeeAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employeeDtos = await _employeeService.GetEmployees();
            return Ok(employeeDtos);
        }

        // GET: api/EmployeeAPI/5
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

        // PUT: api/EmployeeAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/EmployeeAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployeeModel(CreateEmployeeDto dto)
        {
            var resultDto = await _employeeService.CreateEmployee(dto);
            return CreatedAtAction("GetEmployeeModel", new { id = resultDto.EmployeeID }, resultDto);
        }

        // DELETE: api/EmployeeAPI/5
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
