namespace IBDirect.API.DTOs
{
    public class PatientDetails
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Hospital { get; set; }
        public string Diagnosis { get; set; }
        public DateOnly DiagnosisDate { get; set; }
        public bool Stoma { get; set; }
        public string Notes { get; set; }
        public int ConsultantId { get; set; }
        public int NurseId { get; set; }
        public int StomaNurseId { get; set; }
        public int GenpractId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<SurveyDto> Surveys { get; set; } = new();
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
    }
}