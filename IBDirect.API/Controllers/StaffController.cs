using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

[Authorize(Roles = "2,3,4,5")]
public class StaffController : BaseApiController
{
    private readonly DataContext _context;

    public StaffController(DataContext context)
    {
        _context = context;
    }

    private static readonly List<int> validStaffRoleValues = new() { 2, 3, 4, 5 };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetStaff()
    {
        return await _context.Users.Where(u => validStaffRoleValues.Contains(u.Role)).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetStaffMember(int id)
    {
        var staff = await _context.Users.FirstOrDefaultAsync(
            u => u.Id == id && validStaffRoleValues.Contains(u.Role)
        );

        if (staff == null)
        {
            return NotFound("Staff member not found");
        }

        return Ok(staff);
    }

    [HttpGet("my-patients/{staffRole}/{staffId}")]
    public async Task<ActionResult<IEnumerable<PatientDetailsBriefDto>>> GetStaffPatients(
        int staffRole,
        int staffId
    )
    {
        if (!await StaffMemberExists(staffId))
        {
            return NotFound("Staff member not found");
        }

        var filters = new Dictionary<int, Expression<Func<PatientDetails, bool>>>
        {
            { 2, u => u.NurseId == staffId },
            { 3, u => u.StomaNurseId == staffId },
            { 4, u => u.ConsultantId == staffId },
            { 5, u => u.GenpractId == staffId },
        };

        if (!filters.TryGetValue(staffRole, out var filter))
        {
            return BadRequest("Invalid role recieved, please contact an administrator");
        }

        var patients = await _context.PatientDetails
            .Where(filter)
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
        if (!await StaffMemberExists(id))
        {
            return NotFound("Staff member not found");
        }

        if (!await StaffDetailsExists(id))
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        var staffDetails = await _context.StaffDetails.FirstOrDefaultAsync(u => u.StaffId == id);

        return Ok(staffDetails);
    }

    [HttpGet("{id}/my-appointments")]
    public async Task<ActionResult<IEnumerable<StaffAppointmentDto>>> GetMyAppointments(int id)
    {
        if (!await StaffMemberExists(id))
        {
            return NotFound("Staff member not found");
        }

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
        )
        .OrderByDescending(a => a.DateTime)
        .ToListAsync();

        if (appointments == null)
        {
            return NoContent();
        }

        return Ok(appointments);
    }

    [HttpGet("{id}/my-dashboard-hub")]
    public async Task<ActionResult<StaffDashboardHubDto>> GetMyDashboardHub(int id)
    {
        if (!await StaffMemberExists(id))
        {
            return NotFound("Staff member not found");
        }

        if (!await StaffDetailsExists(id))
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        var currentDate = DateTime.UtcNow.Date;
        var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + 1);
        var endOfWeek = startOfWeek.AddDays(6);

        var dashboardHub = new StaffDashboardHubDto
        {
            ThisWeekAppointments = await (
                from a in _context.Appointments
                join p in _context.PatientDetails on a.PatientDetailsId equals p.PatientId
                where a.StaffId == id && a.DateTime >= currentDate && a.DateTime <= endOfWeek
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
            )
            .OrderByDescending(a => a.DateTime)
            .ToListAsync()
        };

        return Ok(dashboardHub);
    }

    [HttpGet("{id}/my-colleagues")]
    public async Task<ActionResult<IEnumerable<StaffDetailsDto>>> GetMyColleagues(int id)
    {
        if (!await StaffMemberExists(id))
        {
            return NotFound("Staff member not found");
        }

        if (!await StaffDetailsExists(id))
        {
            return NotFound("Staff member details not found, please contact an administrator");
        }

        var staffPractice = await _context.StaffDetails
            .Where(u => u.StaffId == id)
            .Select(u => u.Practice)
            .FirstOrDefaultAsync();

        var colleagues = await (
            from s in _context.StaffDetails
            where s.StaffId != id && s.Practice == staffPractice
            select new StaffDetailsDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Role = s.Role,
                Speciality = s.Speciality,
                Practice = s.Practice,
            }
        ).ToListAsync();

        return Ok(colleagues);
    }

    [HttpGet("find-staff/{searchName}")]
    public async Task<ActionResult<IEnumerable<StaffDetailsDto>>> GetStaffByName(string searchName)
    {
        var staffMembers = await _context.StaffDetails
            .Where(u => u.Name.ToLower().Contains(searchName.ToLower()))
            .Select(
                u =>
                    new StaffDetailsDto
                    {
                        StaffId = u.StaffId,
                        Name = u.Name,
                        Role = u.Role,
                        Speciality = u.Speciality,
                        Practice = u.Practice
                    }
            )
            .ToListAsync();

        return Ok(staffMembers);
    }

    private async Task<bool> StaffMemberExists(int id)
    {
        return await _context.Users.AnyAsync(
            u => u.Id == id && validStaffRoleValues.Contains(u.Role)
        );
    }

    private async Task<bool> StaffDetailsExists(int id)
    {
        return await _context.StaffDetails.AnyAsync(u => u.StaffId == id);
    }
}
