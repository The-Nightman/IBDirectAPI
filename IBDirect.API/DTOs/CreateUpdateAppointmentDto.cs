namespace IBDirect.API.DTOs
{
    public class CreateUpdateAppointmentDto
    {
        public int StaffId { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string AppType { get; set; }
        public string Notes { get; set; }
    }
}