using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWAB.Migrations
{
    /// <inheritdoc />
    public partial class Usser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsserId",
                table: "Recruitment",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Recruitment",
                newName: "UsserId");
        }
    }
}
