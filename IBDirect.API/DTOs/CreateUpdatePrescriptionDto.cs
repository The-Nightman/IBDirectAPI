namespace IBDirect.API.DTOs
{
    public class CreateUpdatePrescriptionDto
    {
        public string ScriptName { get; set; }
        public DateOnly ScriptStartDate { get; set; }
        public string ScriptDose { get; set; }
        public string ScriptInterval { get; set; }
        public string ScriptNotes { get; set; }
        public bool ScriptRepeat { get; set; }
        public int PrescribingStaffId { get; set; }
    }
}
