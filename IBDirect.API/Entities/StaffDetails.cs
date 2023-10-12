using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBDirect.API.Entities;

public class StaffDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int StaffId { get; set; }

    public string Name { get; set; }
    public string Role { get; set; }
    public string Speciality { get; set; }
    public string Practice { get; set; }
}
