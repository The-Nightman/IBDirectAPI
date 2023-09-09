using IBDirect.API.Data;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly DataContext _context;
    public PatientsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patients>>> GetPatients()
    {
        var patients = await _context.Patients.ToListAsync();

        return patients;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patients>> GetPatient(int id)
    {
        return await _context.Patients.FindAsync(id);
    }
}