namespace IBDirect.API.DTOs;

public class SeedDataDto
{
    public string Name { get; set; }
    public int Role { get; set; }
    public SeedPatientDetailsDto PatientDetails { get; set; }
    public SeedStaffDetailsDto StaffDetails { get; set; }
}

public class SeedPatientDetailsDto
{
    public int PatientId { get; set; }
    public string Name { get; set; }
    public string Sex { get; set; }
    public string Hospital { get; set; }
    public string Diagnosis { get; set; }
    public DateOnly DiagnosisDate { get; set; }
    public bool Stoma { get; set; }
    public string Notes { get; set; }
    public int ConsultantId { get; set; }
    public int NurseId { get; set; }
    public int? StomaNurseId { get; set; }
    public int GenpractId { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Address { get; set; }
    public List<SeedAppointmentDto> Appointments { get; set; } = new();
    public List<SeedSurveyDto> Surveys { get; set; } = new();
    public List<SeedPrescriptionDto> Prescriptions { get; set; } = new();
}

public class SeedAppointmentDto
{
    public int StaffId { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string AppType { get; set; }
    public string Notes { get; set; }
    public int PatientDetailsId { get; set; }
}

public class SeedSurveyDto
{
    public DateOnly Date { get; set; }
    public int? Q1 { get; set; }
    public int? Q2 { get; set; }
    public int? Q3 { get; set; }
    public int? Q4 { get; set; }
    public bool? Q4a { get; set; }
    public int? Q5 { get; set; }
    public int? Q6 { get; set; }
    public int? Q7 { get; set; }
    public int? Q8 { get; set; }
    public int? Q9 { get; set; }
    public int? Q10 { get; set; }
    public int? Q11 { get; set; }
    public int? Q12 { get; set; }
    public int? ContScore { get; set; }
    public int? Q13 { get; set; }
    public bool Completed { get; set; }
    public int PatientDetailsId { get; set; }
}

public class SeedPrescriptionDto
{
    public string ScriptName { get; set; }
    public DateOnly ScriptStartDate { get; set; }
    public string ScriptDose { get; set; }
    public string ScriptInterval { get; set; }
    public string Notes { get; set; }
    public bool ScriptRepeat { get; set; }
    public int PrescribingStaffId { get; set; }
    public bool Cancelled { get; set; }
    public int PatientDetailsId { get; set; }
}

public class SeedStaffDetailsDto
{
    public int StaffId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string Speciality { get; set; }
    public string Practice { get; set; }
}
