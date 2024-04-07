using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SurveyExtraQuestionsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Q14",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q15",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q16",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q16a",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q17",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q17a",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q18",
                table: "SurveyData");

            migrationBuilder.DropColumn(
                name: "Q19",
                table: "SurveyData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Q14",
                table: "SurveyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Q15",
                table: "SurveyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Q16",
                table: "SurveyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Q16a",
                table: "SurveyData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Q17",
                table: "SurveyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Q17a",
                table: "SurveyData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Q18",
                table: "SurveyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Q19",
                table: "SurveyData",
                type: "integer",
                nullable: true);
        }
    }
}
