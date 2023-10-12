using IBDirect.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<PatientDetails> PatientDetails { get; set; }
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<StaffDetails> StaffDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientDetails>()
            .Property(e => e.Notes)
            .HasMaxLength(2500);
    }
}
