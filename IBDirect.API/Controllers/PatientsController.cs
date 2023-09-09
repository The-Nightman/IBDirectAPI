using IBDirect.API.Data;
using IBDirect.API.Entities;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<Patients>> GetPatients()
    {
        var patients = _context.Patients.ToList();

        return patients;
    }

    [HttpGet("{id}")]
    public ActionResult<Patients> GetPatient(int id)
    {
        return _context.Patients.Find(id);
    }
}