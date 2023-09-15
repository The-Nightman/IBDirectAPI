using System.Security.Cryptography;
using System.Text;
using IBDirect.API.Data;
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
    public async Task<ActionResult<Patients>> Register(string name, string password)
    {
        using var hmac = new HMACSHA512();

        var patient = new Patients
        {
            Name = name,
            PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            Salt = hmac.Key
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return patient;
    }
}