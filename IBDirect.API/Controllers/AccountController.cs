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

    [HttpPost("register/patient")]
    public async Task<ActionResult<Users>> Register(RegisterDto registerDto)
    {
        if (await PatientExists(registerDto.Name)) return BadRequest("Patient already exists");

        using var hmac = new HMACSHA512();

        var patient = new Users
        {
            Name = registerDto.Name.ToLower(),
            PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            Salt = hmac.Key,
            Role = 1
        };

        _context.Users.Add(patient);
        await _context.SaveChangesAsync();

        return patient;
    }

    [HttpPost("register/staff")]
    public async Task<ActionResult<Users>> RegisterStaff(RegisterStaffDto registerDto)
    {
        if (await ValidRoleAsync(registerDto.Role)) return BadRequest("Invalid role");
        if (await StaffExists(registerDto.Name)) return BadRequest("Staff member already exists");

        using var hmac = new HMACSHA512();

        var staff = new Users
        {
            Name = registerDto.Name.ToLower(),
            PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            Salt = hmac.Key,
            Role = registerDto.Role
        };

        _context.Users.Add(staff);
        await _context.SaveChangesAsync();

        return staff;
    }

    [HttpPost("login/patient")]
    public async Task<ActionResult<Users>> LoginPatient(LoginDto loginDto)
    {
        var patient = await _context.Users.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (patient == null || patient.Role != 1) return Unauthorized();

        using var hmac = new HMACSHA512(patient.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != patient.PassHash[i]) return Unauthorized("invalid password");
        }

        return patient;
    }

    [HttpPost("login/staff")]
    public async Task<ActionResult<Users>> LoginStaff(LoginDto loginDto)
    {

        var staff = await _context.Users.SingleOrDefaultAsync(x => x.Name == loginDto.Name);

        if (staff == null || await ValidRoleAsync(staff.Role)) return Unauthorized();

        using var hmac = new HMACSHA512(staff.Salt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        foreach (var (value, i) in computedHash.Select((value, i) => (value, i)))
        {
            if (value != staff.PassHash[i]) return Unauthorized("invalid password");
        }

        return staff;
    }

    private async Task<bool> PatientExists(string name)
    {
        return await _context.Users.AnyAsync(x => x.Name == name.ToLower());
    }

    private async Task<bool> StaffExists(string name)
    {
        return await _context.Users.AnyAsync(x => x.Name == name.ToLower());
    }

    private static Task<bool> ValidRoleAsync(int role)
    {
    List<int> validStaffValues = new() { 2, 3, 4 };
    return Task.FromResult(!validStaffValues.Contains(role));
    }
}