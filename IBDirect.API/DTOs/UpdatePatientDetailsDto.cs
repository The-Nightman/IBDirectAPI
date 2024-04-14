namespace IBDirect.API.DTOs
{
    public class UpdatePatientDetailsDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Diagnosis { get; set; }
        public DateOnly DiagnosisDate { get; set; }
        public bool Stoma { get; set; }
        public int ConsultantId { get; set; }
        public int NurseId { get; set; }
        public int? StomaNurseId { get; set; }
    }
}