using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

[Authorize]
public class StaffController : BaseApiController
{
    private readonly DataContext _context;

    public StaffController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetStaff()
    {
        List<int> validStaffValues = new() { 2, 3, 4, 5 };
        return await _context.Users.Where(u => validStaffValues.Contains(u.Role)).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetStaff(int id)
    {
        List<int> validStaffValues = new() { 2, 3, 4, 5 };
        var staff = await _context.Users.FirstOrDefaultAsync(
            u => u.Id == id && validStaffValues.Contains(u.Role)
        );

        if (staff == null)
        {
            return NotFound("Staff member not found");
        }

        return Ok(staff);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<StaffDetails>> GetStaffDetails(int id)
    {
        var staffDetails = await _context.StaffDetails.FirstOrDefaultAsync(u => u.StaffId == id);

        if (staffDetails == null)
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        return Ok(staffDetails);
    }

    [HttpGet("{id}/myAppointments")]
    public async Task<ActionResult<IEnumerable<StaffAppointmentDto>>> GetMyAppointments(int id)
    {
        var staffDetails = await _context.StaffDetails.FirstOrDefaultAsync(u => u.StaffId == id);

        if (staffDetails == null)
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        return await _context.Appointments
            .Where(a => a.StaffId == id)
            .Include(a => a.PatientDetails)
            .Select(
                a =>
                    new StaffAppointmentDto
                    {
                        Id = a.Id,
                        StaffId = a.StaffId,
                        PatientId = a.PatientDetailsId,
                        PatientName = a.PatientDetails.Name,
                        DateTime = a.DateTime,
                        Location = a.Location,
                        AppType = a.AppType,
                        Notes = a.Notes
                    }
            )
            .ToListAsync();
    }
}
