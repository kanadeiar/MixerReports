using Microsoft.EntityFrameworkCore.Migrations;

namespace MixerReports.lib.Migrations
{
    public partial class addMudAndExperiment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExperiment",
                table: "Mixes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMud",
                table: "Mixes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExperiment",
                table: "Mixes");

            migrationBuilder.DropColumn(
                name: "IsMud",
                table: "Mixes");
        }
    }
}
