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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmployeeShiftAPIController : ControllerBase
    {
        private readonly IEmployeeShiftService _employeeShiftService;

        public EmployeeShiftAPIController(IEmployeeShiftService employeeShiftService)
        {
            _employeeShiftService = employeeShiftService;
        }

        // GET: api/EmployeeShiftAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeShiftModel>>> GetEmployeeShiftModel()
        {
            return Ok(await _employeeShiftService.GetAllEmployeeShiftsAsync());
        }

        // GET: api/EmployeeShiftAPI/5
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

        // PUT: api/EmployeeShiftAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/EmployeeShiftAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeShiftModel>> PostEmployeeShiftModel(EmployeeShiftModel employeeShiftModel)
        {
            var newModel = await _employeeShiftService.CreateEmployeeShift(employeeShiftModel);
            return CreatedAtAction("GetEmployeeShiftModel", new { id = newModel.EmployeeShiftID }, newModel);
        }

        // DELETE: api/EmployeeShiftAPI/5
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

        // POST: api/EmployeeShiftAPI/link
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

        // DELETE: api/EmployeeShiftAPI/unlink?employeeId=1&shiftId=2
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

        // GET: api/EmployeeShiftAPI/employees-in-shift/{shiftId}
        [HttpGet("employees-in-shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesInShift(int shiftId)
        {
            var employeeDtos = await _employeeShiftService.GetEmployeesInShift(shiftId);
            return Ok(employeeDtos);
        }

        // GET: api/EmployeeShiftAPI/shifts-for-employee/{employeeId}
        [HttpGet("shifts-for-employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShiftsForEmployee(int employeeId)
        {
            var shiftDtos = await _employeeShiftService.GetShiftsForEmployee(employeeId);
            return Ok(shiftDtos);
        }
    }
}
