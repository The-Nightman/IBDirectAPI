using IBDirect.API.Data;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Controllers;

[Authorize]
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
        List<int> validStaffValues = new() { 2, 3, 4, 5 };
        var staff = await _context.Users.Where(u => validStaffValues.Contains(u.Role)).ToListAsync();

        return staff;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetStaff(int id)
    {
        List<int> validStaffValues = new() { 2, 3, 4, 5 };
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && validStaffValues.Contains(u.Role));
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<StaffDetails>> GetStaffDetails(int id)
    {
        return await _context.StaffDetails.FirstOrDefaultAsync(u => u.StaffId == id);
    }
}