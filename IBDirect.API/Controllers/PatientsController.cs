using IBDirect.API.Data;
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
}