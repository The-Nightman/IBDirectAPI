using System.ComponentModel.DataAnnotations.Schema;

namespace IBDirect.API.Entities;

[Table("AppointmentData")]
public class Appointment
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string AppType { get; set; }
    public string Notes { get; set; }
}