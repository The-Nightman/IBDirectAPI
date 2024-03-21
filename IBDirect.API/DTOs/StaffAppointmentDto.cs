namespace IBDirect.API.DTOs
{
    public class StaffAppointmentDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public string AppType { get; set; }
        public string Notes { get; set; }
    }
}