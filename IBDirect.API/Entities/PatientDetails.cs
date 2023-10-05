using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBDirect.API.Entities;

public class PatientDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int PatientId { get; set; }

    public string Name { get; set; }
    public string Sex { get; set; }
    public string Hospital { get; set; }
    public string Diagnosis { get; set; }
    public DateOnly DiagnosisDate { get; set; }
    public bool Stoma { get; set; }
    [MaxLength(2500)]
    public string Notes { get; set; }
    public int ConsultantId { get; set; }
    public int NurseId { get; set; }
    public int StomaNurseId { get; set; }
    public int GenpractId { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
    public List<Survey> Surveys { get; set; } = new();
    public List<Prescription> Prescriptions { get; set; } = new();
}
