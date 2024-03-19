namespace IBDirect.API.DTOs
{
    public class StaffDashAppointmentsWithStaffDto
    {
        public StaffAppointmentDto Appointment { get; set; }
        public StaffDetailsDto RelevantConsultant { get; set; }
        public StaffDetailsDto RelevantNurse { get; set; }
        public StaffDetailsDto RelevantStomaNurse { get; set; }
        public StaffDetailsDto RelevantGenpract { get; set; }
    }
}
