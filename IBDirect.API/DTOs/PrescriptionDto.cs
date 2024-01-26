namespace IBDirect.API.DTOs
{
    public class PrescriptionDto
    {
        public string ScriptName { get; set; }
        public DateOnly ScriptStartDate { get; set; }
        public string ScriptDose { get; set; }
        public string ScriptInterval { get; set; }
        public string ScriptNotes { get; set; }
    }
}
