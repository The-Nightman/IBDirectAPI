using System.Security.Cryptography;
using System.Text;
using IBDirect.API.Data;
using IBDirect.API.DTOs;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    public AccountController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("registerPatient")]
    public async Task<ActionResult<Patients>> Register(RegisterDto registerDto)
    {
        if (await PatientExists(registerDto.Name)) return BadRequest("Patient already exists");

        using var hmac = new HMACSHA512();

        var patient = new Patients
        {
            Name = registerDto.Name.ToLower(),
            PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            Salt = hmac.Key
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return patient;
    }

    private async Task<bool> PatientExists(string name)
    {
        return await _context.Patients.AnyAsync(x => x.Name == name.ToLower());
    }

    [HttpPost("registerStaff")]
    public async Task<ActionResult<Staff>> RegisterStaff(RegisterDto registerDto)
    {
        if (await StaffExists(registerDto.Name)) return BadRequest("Staff member already exists");

        using var hmac = new HMACSHA512();

        var staff = new Staff
        {
            Name = registerDto.Name.ToLower(),
            PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            Salt = hmac.Key
        };

        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();

        return staff;
    }

    private async Task<bool> StaffExists(string name)
    {
        return await _context.Staff.AnyAsync(x => x.Name == name.ToLower());
    }

    [HttpPost("loginPatient")]
    public async Task<ActionResult<Patients>> LoginPatient(LoginDto loginDto)
    {
        var patient = await _context.Patients.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (patient == null) return Unauthorized();

        using var hmac = new HMACSHA512(patient.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != patient.PassHash[i]) return Unauthorized("invalid password");
        }

        return patient;
    }

    [HttpPost("loginStaff")]
    public async Task<ActionResult<Staff>> LoginStaff(LoginDto loginDto)
    {
        var staff = await _context.Staff.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (staff == null) return Unauthorized();

        using var hmac = new HMACSHA512(staff.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != staff.PassHash[i]) return Unauthorized("invalid password");
        }

        return staff;
    }
}