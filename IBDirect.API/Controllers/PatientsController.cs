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

    [HttpPost("{id}/addAppointment")]
    public async Task<ActionResult> AddAppointment(
        int id,
        CreateUpdateAppointmentDto appointmentDto
    )
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        Appointment appointment;

        try
        {
            appointment = new Appointment
            {
                StaffId = appointmentDto.StaffId,
                DateTime = appointmentDto.DateTime,
                Location = appointmentDto.Location,
                AppType = appointmentDto.AppType,
                Notes = appointmentDto.Notes,
                PatientDetailsId = id,
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (!await PatientDetailsExists(id))
            {
                return NotFound(
                    "Patient details no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while adding appointments.");
                return StatusCode(
                    500,
                    "An error occurred while adding the appointment, please try again later or contact an administrator"
                );
            }
        }

        return Ok(appointment.Id);
    }

    [HttpPost("{id}/addPrescription")]
    public async Task<ActionResult> AddPrescription(
        int id,
        CreateUpdatePrescriptionDto prescriptionDto
    )
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        Prescription prescription;

        try
        {
            prescription = new Prescription
            {
                ScriptName = prescriptionDto.ScriptName,
                ScriptStartDate = prescriptionDto.ScriptStartDate,
                ScriptDose = prescriptionDto.ScriptDose,
                ScriptInterval = prescriptionDto.ScriptInterval,
                Notes = prescriptionDto.Notes,
                ScriptRepeat = prescriptionDto.ScriptRepeat,
                PrescribingStaffId = prescriptionDto.PrescribingStaffId,
                Cancelled = false,
                PatientDetailsId = id,
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (!await PatientDetailsExists(id))
            {
                return NotFound(
                    "Patient details no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while adding appointments.");
                return StatusCode(
                    500,
                    "An error occurred while adding the prescription, please try again later or contact an administrator"
                );
            }
        }

        return Ok(prescription.Id);
    }

    [HttpPost("{id}/addSurvey")]
    public async Task<ActionResult> AddSurvey(int id, CreateRescheduleSurveyDto surveyDto)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        Survey survey;

        try
        {
            survey = new Survey
            {
                Date = surveyDto.Date,
                PatientDetailsId = id,
                Completed = false,
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (!await PatientDetailsExists(id))
            {
                return NotFound(
                    "Patient details no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while adding appointments.");
                return StatusCode(
                    500,
                    "An error occurred while adding the survey, please try again later or contact an administrator"
                );
            }
        }

        return Ok(survey.Id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetPatients()
    {
        return await _context.Users.Where(user => user.Role == 1).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetPatient(int id)
    {
        var patient = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == 1);

        if (patient == null)
        {
            return NotFound("Patient not found");
        }

        return Ok(patient);
    }

    [HttpGet("findPatient/{searchName}")]
    public async Task<ActionResult<IEnumerable<PatientDetailsBriefDto>>> GetPatientsByName(
        string searchName
    )
    {
        var patients = await _context.PatientDetails
            .Where(p => p.Name.ToLower().Contains(searchName.ToLower()))
            .Select(
                p =>
                    new PatientDetailsBriefDto
                    {
                        PatientId = p.PatientId,
                        Name = p.Name,
                        DateOfBirth = p.DateOfBirth,
                        Diagnosis = p.Diagnosis,
                        Stoma = p.Stoma
                    }
            )
            .ToListAsync();

        return Ok(patients);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<PatientDetailsStaffVDto>> GetPatientDetails(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
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
                Consultant = new StaffDetailsDto
                {
                    StaffId = c.StaffId,
                    Name = c.Name,
                    Role = c.Role,
                    Speciality = c.Speciality,
                    Practice = c.Practice
                },
                Nurse = new StaffDetailsDto
                {
                    StaffId = n.StaffId,
                    Name = n.Name,
                    Role = n.Role,
                    Speciality = n.Speciality,
                    Practice = n.Practice
                },
                StomaNurse =
                    s != null
                        ? new StaffDetailsDto
                        {
                            StaffId = s.StaffId,
                            Name = s.Name,
                            Role = s.Role,
                            Speciality = s.Speciality,
                            Practice = s.Practice
                        }
                        : null,
                Genpract = new StaffDetailsDto
                {
                    StaffId = g.StaffId,
                    Name = g.Name,
                    Role = g.Role,
                    Speciality = g.Speciality,
                    Practice = g.Practice
                },
                DateOfBirth = p.DateOfBirth,
                Address = p.Address,
                Appointments = p.Appointments
                    .Join(
                        _context.StaffDetails,
                        a => a.StaffId,
                        s => s.StaffId,
                        (a, s) =>
                            new AppointmentDto
                            {
                                Id = a.Id,
                                StaffId = a.StaffId,
                                StaffName = s.Name,
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
                                Id = s.Id,
                                Date = s.Date,
                                Q1 = s.Q1,
                                Q2 = s.Q2,
                                Q3 = s.Q3,
                                Q4 = s.Q4,
                                Q4a = s.Q4a ?? false,
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
                                Completed = s.Completed
                            }
                    )
                    .ToList(),
                Prescriptions = p.Prescriptions
                    .Join(
                        _context.StaffDetails,
                        pr => pr.PrescribingStaffId,
                        s => s.StaffId,
                        (pr, s) =>
                            new PrescriptionDto
                            {
                                Id = pr.Id,
                                ScriptName = pr.ScriptName,
                                ScriptStartDate = pr.ScriptStartDate,
                                ScriptDose = pr.ScriptDose,
                                ScriptInterval = pr.ScriptInterval,
                                Notes = pr.Notes,
                                ScriptRepeat = pr.ScriptRepeat,
                                PrescribingStaff = new StaffDetailsDto
                                {
                                    StaffId = s.StaffId,
                                    Name = s.Name,
                                    Role = s.Role,
                                    Speciality = s.Speciality,
                                    Practice = s.Practice
                                },
                                Cancelled = pr.Cancelled,
                            }
                    )
                    .ToList()
            }
        ).FirstOrDefaultAsync();

        return Ok(patientDetails);
    }

    [HttpGet("{id}/myDetailsBrief")]
    public async Task<ActionResult<PatientMyDetailsBriefDto>> GetMyDetailsBrief(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var patientDetails = await (
            from p in _context.PatientDetails
            where p.PatientId == id
            select new PatientMyDetailsBriefDto
            {
                Name = p.Name,
                Diagnosis = p.Diagnosis,
                Hospital = p.Hospital,
            }
        ).FirstOrDefaultAsync();

        return Ok(patientDetails);
    }

    [HttpGet("{id}/myUpcoming")]
    public async Task<ActionResult<PatientUpcomingDto>> GetMyUpcoming(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var upcomingAppointments = await _context.Appointments
            .Where(a => a.PatientDetailsId == id && a.DateTime >= DateTime.UtcNow.AddMinutes(-5))
            .Join(
                _context.StaffDetails,
                a => a.StaffId,
                s => s.StaffId,
                (a, s) =>
                    new AppointmentDto
                    {
                        Id = a.Id,
                        StaffId = a.StaffId,
                        StaffName = s.Name,
                        DateTime = a.DateTime,
                        Location = a.Location,
                        AppType = a.AppType,
                        Notes = a.Notes
                    }
            )
            .ToListAsync();

        var upcomingSurveys = await _context.Surveys
            .Where(s => s.PatientDetailsId == id && s.Completed == false)
            .Select(
                s =>
                    new SurveyDto
                    {
                        Id = s.Id,
                        Date = s.Date,
                        Q1 = s.Q1,
                        Q2 = s.Q2,
                        Q3 = s.Q3,
                        Q4 = s.Q4,
                        Q4a = s.Q4a ?? false,
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
                        Completed = s.Completed
                    }
            )
            .ToListAsync();

        var patientUpcoming = new PatientUpcomingDto
        {
            Appointments = upcomingAppointments,
            Surveys = upcomingSurveys
        };

        return Ok(patientUpcoming);
    }

    [HttpGet("{id}/myDetails")]
    public async Task<ActionResult<PatientMyDetailsDto>> GetMyDetails(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
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
                Consultant = new StaffDetailsDto
                {
                    StaffId = c.StaffId,
                    Name = c.Name,
                    Role = c.Role,
                    Speciality = c.Speciality,
                    Practice = c.Practice
                },
                Nurse = new StaffDetailsDto
                {
                    StaffId = n.StaffId,
                    Name = n.Name,
                    Role = n.Role,
                    Speciality = n.Speciality,
                    Practice = n.Practice
                },
                StomaNurse =
                    s != null
                        ? new StaffDetailsDto
                        {
                            StaffId = s.StaffId,
                            Name = s.Name,
                            Role = s.Role,
                            Speciality = s.Speciality,
                            Practice = s.Practice
                        }
                        : null,
                Genpract = new StaffDetailsDto
                {
                    StaffId = g.StaffId,
                    Name = g.Name,
                    Role = g.Role,
                    Speciality = g.Speciality,
                    Practice = g.Practice
                },
                DateOfBirth = p.DateOfBirth,
                Address = p.Address,
            }
        ).FirstOrDefaultAsync();

        return Ok(patientDetails);
    }

    [HttpGet("{id}/myAppointments")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetMyAppointments(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var appoinments = await _context.Appointments
            .Where(a => a.PatientDetailsId == id)
            .Join(
                _context.StaffDetails,
                a => a.StaffId,
                s => s.StaffId,
                (a, s) =>
                    new AppointmentDto
                    {
                        Id = a.Id,
                        StaffId = a.StaffId,
                        StaffName = s.Name,
                        DateTime = a.DateTime,
                        Location = a.Location,
                        AppType = a.AppType,
                        Notes = a.Notes
                    }
            )
            .ToListAsync();

        return Ok(appoinments);
    }

    [HttpGet("{id}/myPrescriptions")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetMyPrescriptions(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var prescriptions = await _context.Prescriptions
            .Where(pr => pr.PatientDetailsId == id)
            .Join(
                _context.StaffDetails,
                pr => pr.PrescribingStaffId,
                s => s.StaffId,
                (pr, s) =>
                    new PrescriptionDto
                    {
                        Id = pr.Id,
                        ScriptName = pr.ScriptName,
                        ScriptStartDate = pr.ScriptStartDate,
                        ScriptDose = pr.ScriptDose,
                        ScriptInterval = pr.ScriptInterval,
                        Notes = pr.Notes,
                        ScriptRepeat = pr.ScriptRepeat,
                        PrescribingStaff = new StaffDetailsDto
                        {
                            StaffId = s.StaffId,
                            Name = s.Name,
                            Role = s.Role,
                            Speciality = s.Speciality,
                            Practice = s.Practice,
                        },
                        Cancelled = pr.Cancelled,
                    }
            )
            .ToListAsync();

        return Ok(prescriptions);
    }

    [HttpGet("{id}/mySurveys")]
    public async Task<ActionResult<IEnumerable<SurveyDto>>> GetMySurveys(int id)
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var surveys = await _context.Surveys
            .Where(s => s.PatientDetailsId == id)
            .Select(
                s =>
                    new SurveyDto
                    {
                        Id = s.Id,
                        Date = s.Date,
                        Q1 = s.Q1,
                        Q2 = s.Q2,
                        Q3 = s.Q3,
                        Q4 = s.Q4,
                        Q4a = s.Q4a ?? false,
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
                        Completed = s.Completed
                    }
            )
            .ToListAsync();

        return Ok(surveys);
    }

    [HttpPut("updateAppointment/{id}")]
    public async Task<ActionResult> UpdateAppointment(
        int id,
        CreateUpdateAppointmentDto appointmentDto
    )
    {
        if (!await AppointmentExists(id))
        {
            return NotFound("Appointment not found");
        }

        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

        appointment.StaffId = appointmentDto.StaffId;
        appointment.DateTime = appointmentDto.DateTime;
        appointment.Location = appointmentDto.Location;
        appointment.AppType = appointmentDto.AppType;
        appointment.Notes = appointmentDto.Notes;

        _context.Entry(appointment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AppointmentExists(id))
            {
                return NotFound(
                    "Appointment no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the appointment, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPut("updatePrescription/{id}")]
    public async Task<ActionResult> UpdatePrescription(
        int id,
        CreateUpdatePrescriptionDto prescriptionDto
    )
    {
        if (!await PrescriptionExists(id))
        {
            return NotFound("Prescription not found");
        }

        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(pr => pr.Id == id);

        prescription.ScriptName = prescriptionDto.ScriptName;
        prescription.ScriptStartDate = prescriptionDto.ScriptStartDate;
        prescription.ScriptDose = prescriptionDto.ScriptDose;
        prescription.ScriptInterval = prescriptionDto.ScriptInterval;
        prescription.Notes = prescriptionDto.Notes;
        prescription.ScriptRepeat = prescriptionDto.ScriptRepeat;
        prescription.PrescribingStaffId = prescriptionDto.PrescribingStaffId;

        _context.Entry(prescription).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PrescriptionExists(id))
            {
                return NotFound(
                    "Prescription no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the Prescription, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPut("updateSurvey/{id}")]
    public async Task<ActionResult> UpdateSurvey(int id, SurveyDto surveyDto)
    {
        if (!await SurveyExists(id))
        {
            return NotFound("Survey not found");
        }

        var survey = await _context.Surveys.FirstOrDefaultAsync(sur => sur.Id == id);

        survey.Date = surveyDto.Date;
        survey.Q1 = surveyDto.Q1;
        survey.Q2 = surveyDto.Q2;
        survey.Q3 = surveyDto.Q3;
        survey.Q4 = surveyDto.Q4;
        survey.Q4a = surveyDto.Q4a;
        survey.Q5 = surveyDto.Q5;
        survey.Q6 = surveyDto.Q6;
        survey.Q7 = surveyDto.Q7;
        survey.Q8 = surveyDto.Q8;
        survey.Q9 = surveyDto.Q9;
        survey.Q10 = surveyDto.Q10;
        survey.Q11 = surveyDto.Q11;
        survey.Q12 = surveyDto.Q12;
        survey.ContScore = surveyDto.ContScore;
        survey.Q13 = surveyDto.Q13;
        survey.Completed = surveyDto.Completed;

        _context.Entry(survey).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await SurveyExists(id))
            {
                return NotFound(
                    "Survey no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the Survey, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPatch("{id}/updateNotes")]
    public async Task<ActionResult> UpdatePatientNotes(
        int id,
        UpdatePatientNotesDto updatePatientNotesDto
    )
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        var patientDetails = await _context.PatientDetails.FirstOrDefaultAsync(
            p => p.PatientId == id
        );

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
                return NotFound(
                    "Patient details no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the patient details, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPatch("cancelPrescription/{id}")]
    public async Task<ActionResult> CancelPrescription(int id)
    {
        if (!await PrescriptionExists(id))
        {
            return NotFound("Prescription not found");
        }

        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(pr => pr.Id == id);

        prescription.Cancelled = true;

        _context.Entry(prescription).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PrescriptionExists(id))
            {
                return NotFound(
                    "Prescription no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the prescription, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPatch("rescheduleSurvey/{id}")]
    public async Task<ActionResult> RescheduleSurvey(int id, CreateRescheduleSurveyDto surveyDto)
    {
        if (!await SurveyExists(id))
        {
            return NotFound("Survey not found");
        }

        var survey = await _context.Surveys.FirstOrDefaultAsync(sur => sur.Id == id);

        survey.Date = surveyDto.Date;

        _context.Entry(survey).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await SurveyExists(id))
            {
                return NotFound(
                    "Survey no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the Survey, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpPatch("{id}/updateDetails")]
    public async Task<ActionResult> UpdatePatientDetails(
        int id,
        UpdatePatientDetailsDto updatePatientDetailsDto
    )
    {
        if (!await PatientExists(id))
        {
            return NotFound("Patient not found");
        }

        if (!await PatientDetailsExists(id))
        {
            return NotFound("Patient details not found, please contact your administrator");
        }

        if (
            updatePatientDetailsDto.DiagnosisDate > DateOnly.FromDateTime(DateTime.UtcNow)
            || updatePatientDetailsDto.DiagnosisDate.Year < 1900
        )
        {
            return BadRequest("Invalid date, date cannot be in the future or before 1900");
        }

        var patientDetails = await _context.PatientDetails.FirstOrDefaultAsync(
            p => p.PatientId == id
        );

        patientDetails.Name = updatePatientDetailsDto.Name;
        patientDetails.Address = updatePatientDetailsDto.Address;
        patientDetails.Diagnosis = updatePatientDetailsDto.Diagnosis;
        patientDetails.DiagnosisDate = updatePatientDetailsDto.DiagnosisDate;
        patientDetails.Stoma = updatePatientDetailsDto.Stoma;
        patientDetails.ConsultantId = updatePatientDetailsDto.ConsultantId;
        patientDetails.NurseId = updatePatientDetailsDto.NurseId;
        patientDetails.StomaNurseId = updatePatientDetailsDto.StomaNurseId ?? null;

        _context.Entry(patientDetails).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PatientDetailsExists(id))
            {
                return NotFound(
                    "Patient Details no longer exists, if this is unexpected please contact your administrator"
                );
            }
            else
            {
                // TODO: Log error in a method accessible for debugging while dockerized with identifiable string eg _logger.LogError(ex, "An error occurred while updating patient notes.");
                return StatusCode(
                    500,
                    "An error occurred while updating the Patient Details, please try again later or contact an administrator"
                );
            }
        }

        return NoContent();
    }

    [HttpDelete("deleteAppointment/{id}")]
    public async Task<ActionResult> DeleteAppointment(int id)
    {
        if (!await AppointmentExists(id))
        {
            return NotFound("Appointment not found");
        }

        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("deletePrescription/{id}")]
    public async Task<ActionResult> DeletePrescription(int id)
    {
        if (!await PrescriptionExists(id))
        {
            return NotFound("Prescription not found");
        }

        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(pr => pr.Id == id);

        _context.Prescriptions.Remove(prescription);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("deleteSurvey/{id}")]
    public async Task<ActionResult> DeleteSurvey(int id)
    {
        if (!await SurveyExists(id))
        {
            return NotFound("Survey not found");
        }

        var survey = await _context.Surveys.FirstOrDefaultAsync(sur => sur.Id == id);

        _context.Surveys.Remove(survey);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> AppointmentExists(int id)
    {
        return await _context.Appointments.AnyAsync(a => a.Id == id);
    }

    private async Task<bool> PatientDetailsExists(int id)
    {
        return await _context.PatientDetails.AnyAsync(u => u.PatientId == id);
    }

    private async Task<bool> PatientExists(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id && u.Role == 1);
    }

    private async Task<bool> PrescriptionExists(int id)
    {
        return await _context.Prescriptions.AnyAsync(pr => pr.Id == id);
    }

    private async Task<bool> SurveyExists(int id)
    {
        return await _context.Surveys.AnyAsync(sur => sur.Id == id);
    }
}
