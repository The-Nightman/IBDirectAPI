namespace IBDirect.API.DTOs
{
    public class PatientMyStaffDto
    {
        public StaffDetailsDto Consultant { get; set; }
        public StaffDetailsDto Nurse { get; set; }
        public StaffDetailsDto StomaNurse { get; set; }
        public StaffDetailsDto Genpract { get; set; }
    }
}
