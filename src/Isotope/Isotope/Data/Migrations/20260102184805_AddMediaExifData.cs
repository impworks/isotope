using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Isotope.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaExifData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CameraMake",
                table: "Media",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CameraModel",
                table: "Media",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExposureTime",
                table: "Media",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FNumber",
                table: "Media",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FocalLength",
                table: "Media",
                type: "TEXT",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsoSpeed",
                table: "Media",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Media",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LensModel",
                table: "Media",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Media",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraMake",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "CameraModel",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ExposureTime",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FNumber",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FocalLength",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "IsoSpeed",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "LensModel",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Media");
        }
    }
}
