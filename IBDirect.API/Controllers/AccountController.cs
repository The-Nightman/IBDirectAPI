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
}