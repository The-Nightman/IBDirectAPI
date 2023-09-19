using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStaffEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hospital",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "Pass",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "Speciality",
                table: "Staff");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Salt",
                table: "Staff",
                type: "bytea USING \"Salt\"::bytea",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PassHash",
                table: "Staff",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassHash",
                table: "Staff");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "Staff",
                type: "text",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hospital",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pass",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Speciality",
                table: "Staff",
                type: "text",
                nullable: true);
        }
    }
}
