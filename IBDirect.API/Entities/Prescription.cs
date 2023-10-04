using System.ComponentModel.DataAnnotations.Schema;

namespace IBDirect.API.Entities;

[Table("PrescriptionData")]
public class Prescription
{
    public int Id { get; set; }
    public string ScriptName { get; set; }
    public string ScriptDose { get; set; }
    public string ScriptInterval { get; set; }
    public string ScriptNotes { get; set; }
}