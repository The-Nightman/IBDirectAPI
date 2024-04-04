namespace IBDirect.API.DTOs
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public string ScriptName { get; set; }
        public DateOnly ScriptStartDate { get; set; }
        public string ScriptDose { get; set; }
        public string ScriptInterval { get; set; }
        public string Notes { get; set; }
        public bool ScriptRepeat { get; set; }
        public StaffDetailsDto PrescribingStaff { get; set; }
        public bool Cancelled { get; set; }
    }
}
