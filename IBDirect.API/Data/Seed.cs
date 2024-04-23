using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IBDirect.API.Data;
using IBDirect.API.Entities;
using IBDirect.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<SeedDataDto>>(usersData, options);

        foreach (var user in users)
        {
            using var hmac = new HMACSHA512();


            var newUser = new Users
            {
                Name = user.Name,
                PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password")),
                Salt = hmac.Key,
                Role = user.Role
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            if (user.PatientDetails != null)
            {
                string[] splitName = user.PatientDetails.Name.Split(' ');
                Array.Reverse(splitName);
                string generatedName = string.Join(", ", splitName);

                var patientDetails = new PatientDetails
                {
                    PatientId = newUser.Id,
                    Name = generatedName,
                    Sex = user.PatientDetails.Sex,
                    Hospital = user.PatientDetails.Hospital,
                    Diagnosis = user.PatientDetails.Diagnosis,
                    DiagnosisDate = user.PatientDetails.DiagnosisDate,
                    Stoma = user.PatientDetails.Stoma,
                    Notes = user.PatientDetails.Notes,
                    ConsultantId = user.PatientDetails.ConsultantId,
                    NurseId = user.PatientDetails.NurseId,
                    StomaNurseId = user.PatientDetails.StomaNurseId,
                    GenpractId = user.PatientDetails.GenpractId,
                    DateOfBirth = user.PatientDetails.DateOfBirth,
                    Address = user.PatientDetails.Address,
                    Appointments = user.PatientDetails.Appointments
                        .Select(
                            a =>
                                new Appointment
                                {
                                    StaffId = a.StaffId,
                                    DateTime = a.DateTime,
                                    Location = a.Location,
                                    AppType = a.AppType,
                                    Notes = a.Notes,
                                    PatientDetailsId = newUser.Id
                                }
                        )
                        .ToList(),
                    Surveys = user.PatientDetails.Surveys
                        .Select(
                            s =>
                                new Survey
                                {
                                    Date = s.Date,
                                    Q1 = s.Q1,
                                    Q2 = s.Q2,
                                    Q3 = s.Q3,
                                    Q4 = s.Q4,
                                    Q4a = s.Q4a,
                                    Q5 = s.Q5,
                                    Q6 = s.Q6,
                                    Q7 = s.Q7,
                                    Q8 = s.Q8,
                                    Q9 = s.Q9,
                                    Q10 = s.Q10,
                                    Q11 = s.Q11,
                                    Q12 = s.Q12,
                                    ContScore = s.ContScore,
                                    Q13 = s.Q13,
                                    Completed = s.Completed,
                                    PatientDetailsId = newUser.Id
                                }
                        )
                        .ToList(),
                    Prescriptions = user.PatientDetails.Prescriptions
                        .Select(
                            p =>
                                new Prescription
                                {
                                    ScriptName = p.ScriptName,
                                    ScriptStartDate = p.ScriptStartDate,
                                    ScriptDose = p.ScriptDose,
                                    ScriptInterval = p.ScriptInterval,
                                    Notes = p.Notes,
                                    ScriptRepeat = p.ScriptRepeat,
                                    PrescribingStaffId = p.PrescribingStaffId,
                                    Cancelled = p.Cancelled,
                                    PatientDetailsId = newUser.Id
                                }
                        )
                        .ToList()
                };

                context.PatientDetails.Add(patientDetails);
            }
            else if (user.StaffDetails != null)
            {
                var staff = new StaffDetails
                {
                    StaffId = newUser.Id,
                    Name = user.StaffDetails.Name,
                    Role = user.StaffDetails.Role,
                    Speciality = user.StaffDetails.Speciality,
                    Practice = user.StaffDetails.Practice
                };

                context.StaffDetails.Add(staff);
            }
        }
        await context.SaveChangesAsync();
    }
}
