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

    [HttpGet("mypatients/{staffRole}/{staffId}")]
    public async Task<ActionResult<IEnumerable<PatientDetailsBriefDto>>> GetStaffPatients(
        int staffRole,
        int staffId
    )
    {
        IQueryable<PatientDetails> query = _context.PatientDetails;

        switch (staffRole)
        {
            case 2:
                query = query.Where(u => u.NurseId == staffId);
                break;

            case 3:
                query = query.Where(u => u.StomaNurseId == staffId);
                break;

            case 4:
                query = query.Where(u => u.ConsultantId == staffId);
                break;

            case 5:
                query = query.Where(u => u.GenpractId == staffId);
                break;

            default:
                return BadRequest();
        }

        var patients = await query
            .Select(
                u =>
                    new PatientDetailsBriefDto
                    {
                        PatientId = u.PatientId,
                        Name = u.Name,
                        DateOfBirth = u.DateOfBirth,
                        Diagnosis = u.Diagnosis,
                        Stoma = u.Stoma
                    }
            )
            .ToListAsync();

        if (!patients.Any())
        {
            return NoContent();
        }

        return Ok(patients);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<StaffDetailsDto>> GetStaffDetails(int id)
    {
        if (!await StaffDetailsExists(id))
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        var staffDetails = await _context.StaffDetails.FirstOrDefaultAsync(u => u.StaffId == id);

        return Ok(staffDetails);
    }

    [HttpGet("{id}/myAppointments")]
    public async Task<ActionResult<IEnumerable<StaffAppointmentDto>>> GetMyAppointments(int id)
    {
        if (!await StaffDetailsExists(id))
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        var appointments = await (
            from a in _context.Appointments
            join p in _context.PatientDetails on a.PatientDetailsId equals p.PatientId
            where a.StaffId == id
            select new StaffAppointmentDto
            {
                Id = a.Id,
                StaffId = a.StaffId,
                PatientId = p.PatientId,
                PatientName = p.Name,
                DateTime = a.DateTime,
                Location = a.Location,
                AppType = a.AppType,
                Notes = a.Notes
            }
        ).ToListAsync();

        if (appointments == null)
        {
            return NoContent();
        }

        return Ok(appointments);
    }

    private async Task<bool> StaffDetailsExists(int id)
    {
        return await _context.StaffDetails.AnyAsync(u => u.StaffId == id);
    }
}
