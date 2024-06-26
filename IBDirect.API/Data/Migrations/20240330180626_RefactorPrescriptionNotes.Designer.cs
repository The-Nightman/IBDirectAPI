﻿// <auto-generated />
using System;
using IBDirect.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240330180626_RefactorPrescriptionNotes")]
    partial class RefactorPrescriptionNotes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IBDirect.API.Entities.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AppType")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("PatientDetailsId")
                        .HasColumnType("integer");

                    b.Property<int>("StaffId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PatientDetailsId");

                    b.ToTable("AppointmentData");
                });

            modelBuilder.Entity("IBDirect.API.Entities.PatientDetails", b =>
                {
                    b.Property<int>("PatientId")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<int>("ConsultantId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Diagnosis")
                        .HasColumnType("text");

                    b.Property<DateOnly>("DiagnosisDate")
                        .HasColumnType("date");

                    b.Property<int>("GenpractId")
                        .HasColumnType("integer");

                    b.Property<string>("Hospital")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasMaxLength(2500)
                        .HasColumnType("character varying(2500)");

                    b.Property<int>("NurseId")
                        .HasColumnType("integer");

                    b.Property<string>("Sex")
                        .HasColumnType("text");

                    b.Property<bool>("Stoma")
                        .HasColumnType("boolean");

                    b.Property<int?>("StomaNurseId")
                        .HasColumnType("integer");

                    b.HasKey("PatientId");

                    b.ToTable("PatientDetails");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Prescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Cancelled")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int>("PatientDetailsId")
                        .HasColumnType("integer");

                    b.Property<int>("PrescribingStaffId")
                        .HasColumnType("integer");

                    b.Property<string>("ScriptDose")
                        .HasColumnType("text");

                    b.Property<string>("ScriptInterval")
                        .HasColumnType("text");

                    b.Property<string>("ScriptName")
                        .HasColumnType("text");

                    b.Property<bool>("ScriptRepeat")
                        .HasColumnType("boolean");

                    b.Property<DateOnly>("ScriptStartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("PatientDetailsId");

                    b.ToTable("PrescriptionData");
                });

            modelBuilder.Entity("IBDirect.API.Entities.StaffDetails", b =>
                {
                    b.Property<int>("StaffId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Practice")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<string>("Speciality")
                        .HasColumnType("text");

                    b.HasKey("StaffId");

                    b.ToTable("StaffDetails");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Survey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ContScore")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PatientDetailsId")
                        .HasColumnType("integer");

                    b.Property<int?>("Q1")
                        .HasColumnType("integer");

                    b.Property<int?>("Q10")
                        .HasColumnType("integer");

                    b.Property<int?>("Q11")
                        .HasColumnType("integer");

                    b.Property<int?>("Q12")
                        .HasColumnType("integer");

                    b.Property<int?>("Q13")
                        .HasColumnType("integer");

                    b.Property<int?>("Q14")
                        .HasColumnType("integer");

                    b.Property<int?>("Q15")
                        .HasColumnType("integer");

                    b.Property<int?>("Q16")
                        .HasColumnType("integer");

                    b.Property<string>("Q16a")
                        .HasColumnType("text");

                    b.Property<int?>("Q17")
                        .HasColumnType("integer");

                    b.Property<int?>("Q18")
                        .HasColumnType("integer");

                    b.Property<int?>("Q19")
                        .HasColumnType("integer");

                    b.Property<int?>("Q2")
                        .HasColumnType("integer");

                    b.Property<int?>("Q3")
                        .HasColumnType("integer");

                    b.Property<int?>("Q4")
                        .HasColumnType("integer");

                    b.Property<bool?>("Q4a")
                        .HasColumnType("boolean");

                    b.Property<int?>("Q5")
                        .HasColumnType("integer");

                    b.Property<int?>("Q6")
                        .HasColumnType("integer");

                    b.Property<int?>("Q7")
                        .HasColumnType("integer");

                    b.Property<int?>("Q8")
                        .HasColumnType("integer");

                    b.Property<int?>("Q9")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PatientDetailsId");

                    b.ToTable("SurveyData");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<byte[]>("PassHash")
                        .HasColumnType("bytea");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Salt")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Appointment", b =>
                {
                    b.HasOne("IBDirect.API.Entities.PatientDetails", "PatientDetails")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PatientDetails");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Prescription", b =>
                {
                    b.HasOne("IBDirect.API.Entities.PatientDetails", "PatientDetails")
                        .WithMany("Prescriptions")
                        .HasForeignKey("PatientDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PatientDetails");
                });

            modelBuilder.Entity("IBDirect.API.Entities.Survey", b =>
                {
                    b.HasOne("IBDirect.API.Entities.PatientDetails", "PatientDetails")
                        .WithMany("Surveys")
                        .HasForeignKey("PatientDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PatientDetails");
                });

            modelBuilder.Entity("IBDirect.API.Entities.PatientDetails", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Prescriptions");

                    b.Navigation("Surveys");
                });
#pragma warning restore 612, 618
        }
    }
}
