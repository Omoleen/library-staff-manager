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
    /// API controller for managing work shifts through RESTful endpoints.
    /// Provides CRUD operations for shift management through API calls.
    /// This controller requires Admin role authorization for all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ShiftAPIController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        /// <summary>
        /// Initializes a new instance of the ShiftAPIController.
        /// </summary>
        /// <param name="shiftService">The service for managing shift data</param>
        public ShiftAPIController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        /// <summary>
        /// Retrieves all shifts in the system.
        /// </summary>
        /// <returns>A list of all shifts with their details</returns>
        /// <response code="200">Returns the list of shifts</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetShifts()
        {
            var shiftDtos = await _shiftService.GetShifts();
            return Ok(shiftDtos);
        }

        /// <summary>
        /// Retrieves a specific shift by its ID.
        /// </summary>
        /// <param name="id">The ID of the shift to retrieve</param>
        /// <returns>The shift details</returns>
        /// <response code="200">Returns the requested shift</response>
        /// <response code="404">If the shift is not found</response>
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

        /// <summary>
        /// Updates an existing shift's information.
        /// </summary>
        /// <param name="id">The ID of the shift to update</param>
        /// <param name="dto">The updated shift information</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the shift was successfully updated</response>
        /// <response code="404">If the shift is not found</response>
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

        /// <summary>
        /// Creates a new shift in the system.
        /// </summary>
        /// <param name="dto">The shift information to create</param>
        /// <returns>The newly created shift</returns>
        /// <response code="201">Returns the newly created shift</response>
        /// <response code="400">If the shift data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<ShiftDto>> PostShiftModel(CreateShiftDto dto)
        {
            var resultDto = await _shiftService.CreateShift(dto);
            return CreatedAtAction("GetShiftModel", new { id = resultDto.ShiftID }, resultDto);
        }

        /// <summary>
        /// Deletes a specific shift from the system.
        /// </summary>
        /// <param name="id">The ID of the shift to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the shift was successfully deleted</response>
        /// <response code="404">If the shift is not found</response>
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
