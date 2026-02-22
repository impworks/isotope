using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Isotope.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedLinkVisitCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "SharedLinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "SharedLinks");
        }
    }
}
