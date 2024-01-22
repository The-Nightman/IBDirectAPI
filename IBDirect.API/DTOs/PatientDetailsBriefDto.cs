namespace IBDirect.API.DTOs
{
    public class PatientDetailsBriefDto
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Diagnosis { get; set; }
        public bool Stoma { get; set; }
    }
}