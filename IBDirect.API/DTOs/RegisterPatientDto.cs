using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IBDirect.API.DTOs
{
    public class RegisterPatientDto
    {
        [Required]
        public string Firstname { get; set; }
        
        [Required]
        public string Surname { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Sex { get; set; }
        
        [Required]
        public string Hospital { get; set; }
        
        [Required]
        public string Diagnosis { get; set; }
        
        [Required]
        public DateOnly DiagnosisDate { get; set; }
        
        [Required]
        public bool Stoma { get; set; }
        
        [Required]
        public int ConsultantId { get; set; }
        
        [Required]
        public int NurseId { get; set; }
        
        public int? StomaNurseId { get; set; }
        
        [Required]
        public int GenpractId { get; set; }
        
        [Required]
        public string Address { get; set; }

        [JsonIgnore]
        public List<AppointmentDto> Appointments { get; set; } = new();
        [JsonIgnore]
        public List<SurveyDto> Surveys { get; set; } = new();
        [JsonIgnore]
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
    }
}