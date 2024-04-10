namespace IBDirect.API.DTOs
{
    public class PatientUpcomingDto
    {
        public List<AppointmentDto> Appointments { get; set; }
        public List<SurveyDto> Surveys { get; set; }
    }
}
