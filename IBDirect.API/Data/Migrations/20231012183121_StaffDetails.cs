using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IBDirect.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class StaffDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffDetails",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Speciality = table.Column<string>(type: "text", nullable: false),
                    Practice = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDetails", x => x.StaffId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffDetails");
        }
    }
}
