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
        var patient = await _context.Users.FindAsync(id);

        if (patient == null)
        {
            return NotFound("Patient not found");
        }

        var patientDetails = await (
            from p in _context.PatientDetails
            join c in _context.StaffDetails on p.ConsultantId equals c.StaffId
            join n in _context.StaffDetails on p.NurseId equals n.StaffId
            from s in _context.StaffDetails.Where(s => p.StomaNurseId == s.StaffId).DefaultIfEmpty()
            join g in _context.StaffDetails on p.GenpractId equals g.StaffId
            where p.PatientId == id
            select new PatientDetailsStaffVDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Sex = p.Sex,
                Hospital = p.Hospital,
                Diagnosis = p.Diagnosis,
                DiagnosisDate = p.DiagnosisDate,
                Stoma = p.Stoma,
                Notes = p.Notes,
                ConsultantName = c.Name,
                NurseName = n.Name,
                StomaNurseName = s != null ? s.Name : null,
                GenpractName = g.Name,
                DateOfBirth = p.DateOfBirth,
                Address = p.Address,
                Appointments = p.Appointments
                    .Select(
                        a =>
                            new AppointmentDto
                            {
                                StaffId = a.StaffId,
                                DateTime = a.DateTime,
                                Location = a.Location,
                                AppType = a.AppType,
                                Notes = a.Notes
                            }
                    )
                    .ToList(),
                Surveys = p.Surveys
                    .Select(
                        s =>
                            new SurveyDto
                            {
                                DateTime = s.DateTime,
                                Q1 = s.Q1,
                                Q2 = s.Q2,
                                Q3 = s.Q3,
                                Q4 = s.Q4,
                                Q5 = s.Q5,
                                Q6 = s.Q6,
                                Q7 = s.Q7,
                                Q8 = s.Q8,
                                Q9 = s.Q9,
                                Q10 = s.Q10,
                                Q11 = s.Q11,
                                Q12 = s.Q12,
                                ContScore = s.ContScore,
                                Q13 = s.Q13,
                                Q14 = s.Q14,
                                Q15 = s.Q15,
                                Q16 = s.Q16,
                                Q16a = s.Q16a,
                                Q17 = s.Q17,
                                Q18 = s.Q18,
                                Q19 = s.Q19
                            }
                    )
                    .ToList(),
                Prescriptions = p.Prescriptions
                    .Select(
                        p =>
                            new PrescriptionDto
                            {
                                ScriptName = p.ScriptName,
                                ScriptStartDate = p.ScriptStartDate,
                                ScriptDose = p.ScriptDose,
                                ScriptInterval = p.ScriptInterval,
                                ScriptNotes = p.ScriptNotes
                            }
                    )
                    .ToList()
            }
        ).FirstOrDefaultAsync();

        if (patientDetails == null)
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        return Ok(patientDetails);
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

    [HttpPut("{id}/updateNotes")]
    public async Task<ActionResult> UpdatePatientNotes(
        int id,
        UpdatePatientNotesDto updatePatientNotesDto
    )
    {
        var patientDetails = await _context.PatientDetails.FirstOrDefaultAsync(
            p => p.PatientId == id
        );

        if (patientDetails == null)
        {
            return NotFound("Patient not found");
        }

        patientDetails.Notes = updatePatientNotesDto.Notes;

        _context.Entry(patientDetails).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PatientDetailsExists(id))
            {
                return NotFound("Patient details no longer exists");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private async Task<bool> PatientDetailsExists(int id)
    {
        return await _context.PatientDetails.AnyAsync(u => u.PatientId == id);
    }
}
