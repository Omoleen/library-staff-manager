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
    public class ShiftAPIController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftAPIController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        // GET: api/ShiftAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShifts()
        {
            var shiftDtos = await _shiftService.GetShifts();
            return Ok(shiftDtos);
        }

        // GET: api/ShiftAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftDto>> GetShiftModel(int id)
        {
            var dto = await _shiftService.GetShift(id);
            if (dto == null)
            {
                return NotFound();
            }
            return Ok(dto);
        }

        // PUT: api/ShiftAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShiftModel(int id, UpdateShiftDto dto)
        {
            var result = await _shiftService.UpdateShift(id, dto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST: api/ShiftAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShiftDto>> PostShiftModel(CreateShiftDto dto)
        {
            var resultDto = await _shiftService.CreateShift(dto);
            return CreatedAtAction("GetShiftModel", new { id = resultDto.ShiftID }, resultDto);
        }

        // DELETE: api/ShiftAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShiftModel(int id)
        {
            var result = await _shiftService.DeleteShift(id);
            if(!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
