namespace IBDirect.API.DTOs
{
    public class PatientDetailsStaffVDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Hospital { get; set; }
        public string Diagnosis { get; set; }
        public DateOnly DiagnosisDate { get; set; }
        public bool Stoma { get; set; }
        public string Notes { get; set; }
        public string ConsultantName { get; set; }
        public string NurseName { get; set; }
        public string StomaNurseName { get; set; }
        public string GenpractName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<SurveyDto> Surveys { get; set; } = new();
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
    }
}