using System.ComponentModel.DataAnnotations.Schema;

namespace IBDirect.API.Entities;

[Table("PrescriptionData")]
public class Prescription
{
    public int Id { get; set; }
    public string ScriptName { get; set; }
    public DateOnly ScriptStartDate { get; set; }
    public string ScriptDose { get; set; }
    public string ScriptInterval { get; set; }
    public string ScriptNotes { get; set; }
    public bool ScriptRepeat { get; set; }
    public int PrescribingStaffId { get; set; }

    public int PatientDetailsId { get; set; }
    public PatientDetails PatientDetails { get; set; }
}