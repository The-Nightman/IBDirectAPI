using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientDetails",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Sex = table.Column<string>(type: "text", nullable: false),
                    Hospital = table.Column<string>(type: "text", nullable: false),
                    Diagnosis = table.Column<string>(type: "text", nullable: false),
                    DiagnosisDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Stoma = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2500)", maxLength: 2500, nullable: true),
                    ConsultantId = table.Column<int>(type: "integer", nullable: false),
                    NurseId = table.Column<int>(type: "integer", nullable: false),
                    StomaNurseId = table.Column<int>(type: "integer", nullable: true),
                    GenpractId = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDetails", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    AppType = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    PatientDetailsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentData_PatientDetails_PatientDetailsId",
                        column: x => x.PatientDetailsId,
                        principalTable: "PatientDetails",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScriptName = table.Column<string>(type: "text", nullable: false),
                    ScriptStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScriptDose = table.Column<string>(type: "text", nullable: false),
                    ScriptInterval = table.Column<string>(type: "text", nullable: false),
                    ScriptNotes = table.Column<string>(type: "text", nullable: true),
                    PatientDetailsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionData_PatientDetails_PatientDetailsId",
                        column: x => x.PatientDetailsId,
                        principalTable: "PatientDetails",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Q1 = table.Column<int>(type: "integer", nullable: false),
                    Q2 = table.Column<int>(type: "integer", nullable: false),
                    Q3 = table.Column<int>(type: "integer", nullable: false),
                    Q4 = table.Column<int>(type: "integer", nullable: false),
                    Q4a = table.Column<bool>(type: "boolean", nullable: false),
                    Q5 = table.Column<int>(type: "integer", nullable: false),
                    Q6 = table.Column<int>(type: "integer", nullable: false),
                    Q7 = table.Column<int>(type: "integer", nullable: false),
                    Q8 = table.Column<int>(type: "integer", nullable: false),
                    Q9 = table.Column<int>(type: "integer", nullable: false),
                    Q10 = table.Column<int>(type: "integer", nullable: false),
                    Q11 = table.Column<int>(type: "integer", nullable: false),
                    Q12 = table.Column<int>(type: "integer", nullable: false),
                    ContScore = table.Column<int>(type: "integer", nullable: false),
                    Q13 = table.Column<int>(type: "integer", nullable: false),
                    Q14 = table.Column<int>(type: "integer", nullable: false),
                    Q15 = table.Column<int>(type: "integer", nullable: false),
                    Q16 = table.Column<int>(type: "integer", nullable: false),
                    Q16a = table.Column<string>(type: "text", nullable: true),
                    Q17 = table.Column<int>(type: "integer", nullable: false),
                    Q18 = table.Column<int>(type: "integer", nullable: false),
                    Q19 = table.Column<int>(type: "integer", nullable: false),
                    PatientDetailsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyData_PatientDetails_PatientDetailsId",
                        column: x => x.PatientDetailsId,
                        principalTable: "PatientDetails",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentData_PatientDetailsId",
                table: "AppointmentData",
                column: "PatientDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionData_PatientDetailsId",
                table: "PrescriptionData",
                column: "PatientDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyData_PatientDetailsId",
                table: "SurveyData",
                column: "PatientDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentData");

            migrationBuilder.DropTable(
                name: "PrescriptionData");

            migrationBuilder.DropTable(
                name: "SurveyData");

            migrationBuilder.DropTable(
                name: "PatientDetails");
        }
    }
}
