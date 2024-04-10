namespace IBDirect.API.DTOs
{
    public class PatientMyDetailsDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Hospital { get; set; }
        public string Diagnosis { get; set; }
        public DateOnly DiagnosisDate { get; set; }
        public bool Stoma { get; set; }
        public string Notes { get; set; }
        public StaffDetailsDto Consultant { get; set; }
        public StaffDetailsDto Nurse { get; set; }
        public StaffDetailsDto StomaNurse { get; set; }
        public StaffDetailsDto Genpract { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
    }
}
