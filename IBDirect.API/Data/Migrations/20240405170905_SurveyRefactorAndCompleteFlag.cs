using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SurveyRefactorAndCompleteFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "SurveyData");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "SurveyData",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "SurveyData",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SurveyData");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "SurveyData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
