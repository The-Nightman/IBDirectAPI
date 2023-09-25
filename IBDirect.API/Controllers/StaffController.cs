using IBDirect.API.Data;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

public class StaffController : BaseApiController
{
    private readonly DataContext _context;
    public StaffController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Users>>> GetStaff()
    {
        var staff = await _context.Users.Where(u => u.Role == 2 || u.Role == 3 || u.Role == 4).ToListAsync();

        return staff;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetStaff(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && (u.Role == 2 || u.Role == 3 || u.Role == 4));
    }
}