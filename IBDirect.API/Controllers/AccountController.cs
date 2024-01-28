using System.Security.Cryptography;
using System.Text;
using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using IBDirect.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register/patient")]
    public async Task<ActionResult<UserDto>> Register(RegisterPatientDto regPatientDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            string generatedName = await GeneratePatientUsername(
                regPatientDto.Firstname,
                regPatientDto.Surname,
                regPatientDto.DateOfBirth
            );
            if (await PatientExists(generatedName))
                return BadRequest("Patient already exists");

            if (regPatientDto.Stoma == true && regPatientDto.StomaNurseId == null)
                return BadRequest("Stoma Patient requires a Stoma Nurse");

            using var hmac = new HMACSHA512();

            var patient = new Users
            {
                Name = generatedName,
                PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regPatientDto.Password)),
                Salt = hmac.Key,
                Role = 1
            };

            _context.Users.Add(patient);
            await _context.SaveChangesAsync();

            var patientDetails = new PatientDetails
            {
                PatientId = patient.Id,
                Name = $"{regPatientDto.Surname}, {regPatientDto.Firstname}",
                Sex = regPatientDto.Sex,
                Hospital = regPatientDto.Hospital,
                Diagnosis = regPatientDto.Diagnosis,
                DiagnosisDate = regPatientDto.DiagnosisDate,
                Stoma = regPatientDto.Stoma,
                ConsultantId = regPatientDto.ConsultantId,
                NurseId = regPatientDto.NurseId,
                StomaNurseId = regPatientDto.StomaNurseId,
                GenpractId = regPatientDto.GenpractId,
                DateOfBirth = regPatientDto.DateOfBirth,
                Address = regPatientDto.Address,
            };

            _context.PatientDetails.Add(patientDetails);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UserDto { Name = patient.Name, Token = _tokenService.CreateToken(patient) };
        }
        catch (Exception)
        {
            //setup logging to console so errors can be debugged when dockerized ex.Message
            await transaction.RollbackAsync();
            return StatusCode(500, "A Transaction error has occurred, contact an administrator");
        }
    }

    [HttpPost("register/staff")]
    public async Task<ActionResult<UserDto>> RegisterStaff(RegisterStaffDto registerDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (!await ValidRoleAsync(registerDto.Role))
                return BadRequest("Invalid role");
            if (await StaffExists(registerDto.Username))
                return BadRequest("Staff member already exists");

            using var hmac = new HMACSHA512();

            var staff = new Users
            {
                Name = registerDto.Username,
                PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                Salt = hmac.Key,
                Role = registerDto.Role
            };

            _context.Users.Add(staff);
            await _context.SaveChangesAsync();

            string roleName = await StaffRoleIdToString(registerDto.Role);

            var staffDetails = new StaffDetails
            {
                StaffId = staff.Id,
                Name = registerDto.Name,
                Role = roleName,
                Speciality = registerDto.Specialty,
                Practice = registerDto.Practice
            };

            _context.StaffDetails.Add(staffDetails);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UserDto { Name = staff.Name, Token = _tokenService.CreateToken(staff) };
        }
        catch (Exception)
        {
            //setup logging to console so errors can be debugged when dockerized ex.Message
            await transaction.RollbackAsync();
            return StatusCode(500, "A Transaction error has occurred, contact an administrator");
        }
    }

    [HttpPost("login/patient")]
    public async Task<ActionResult<UserDto>> LoginPatient(LoginDto loginDto)
    {
        var patient = await _context.Users.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (patient == null || patient.Role != 1)
            return Unauthorized("Invalid details or password");

        using var hmac = new HMACSHA512(patient.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != patient.PassHash[i])
                return Unauthorized("Invalid details or password");
        }

        return new UserDto { Name = patient.Name, Token = _tokenService.CreateToken(patient) };
    }

    [HttpPost("login/staff")]
    public async Task<ActionResult<UserDto>> LoginStaff(LoginDto loginDto)
    {
        var staff = await _context.Users.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (staff == null || !await ValidRoleAsync(staff.Role))
            return Unauthorized("Invalid username or password");

        using var hmac = new HMACSHA512(staff.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != staff.PassHash[i])
                return Unauthorized("Invalid username or password");
        }

        return new UserDto { Name = staff.Name, Token = _tokenService.CreateToken(staff) };
    }

    private static Task<string> GeneratePatientUsername(
        string firstname,
        string surname,
        DateOnly dob
    )
    {
        string username = $"{firstname[..1]}{surname}{dob:ddMMyyyy}";
        return Task.FromResult(username);
    }

    private async Task<bool> PatientExists(string name)
    {
        return await _context.Users.AnyAsync(x => x.Name == name);
    }

    private async Task<bool> StaffExists(string name)
    {
        return await _context.Users.AnyAsync(x => x.Name == name);
    }

    private static Task<bool> ValidRoleAsync(int role)
    {
        List<int> validStaffValues = new() { 2, 3, 4, 5 };
        return Task.FromResult(validStaffValues.Contains(role));
    }

    private static Task<string> StaffRoleIdToString(int roleId)
    {
        string roleName = null;
        switch (roleId)
        {
            case 2:
                roleName = "Nurse";
                break;
            case 3:
                roleName = "Stoma Nurse";
                break;
            case 4:
                roleName = "Consultant";
                break;
            case 5:
                roleName = "General Practitioner";
                break;
        }
        return Task.FromResult(roleName);
    }
}
