using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

[Authorize]
public class PatientsController : BaseApiController
{
    private readonly DataContext _context;

    public PatientsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetPatients()
    {
        var patients = await _context.Users.Where(user => user.Role == 1).ToListAsync();

        return patients;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetPatient(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == 1);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<PatientDetails>> GetPatientDetails(int id)
    {
        return await _context.PatientDetails.FirstOrDefaultAsync(u => u.PatientId == id);
    }

    [HttpGet("mypatients/{staffRole}/{staffId}")]
    public async Task<ActionResult<PatientDetails>> GetStaffPatients(int staffRole, int staffId)
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

        if (patients == null || !patients.Any())
        {
            return NoContent();
        }

        return Ok(patients);
    }
}
