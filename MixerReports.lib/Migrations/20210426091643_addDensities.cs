using Microsoft.EntityFrameworkCore.Migrations;

namespace MixerReports.lib.Migrations
{
    public partial class addDensities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "DensityRevertMud",
                table: "Mixes",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DensitySandMud",
                table: "Mixes",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DensityRevertMud",
                table: "Mixes");

            migrationBuilder.DropColumn(
                name: "DensitySandMud",
                table: "Mixes");
        }
    }
}
