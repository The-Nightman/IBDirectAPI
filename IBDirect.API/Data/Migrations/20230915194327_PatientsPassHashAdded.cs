using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatientsPassHashAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pass",
                table: "Patients");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "Patients",
                type: "bytea USING \"Salt\"::bytea",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PassHash",
                table: "Patients",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassHash",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pass",
                table: "Patients",
                type: "text",
                nullable: true);
        }
    }
}
