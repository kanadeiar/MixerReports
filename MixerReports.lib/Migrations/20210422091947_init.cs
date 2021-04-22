using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MixerReports.lib.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormNumber = table.Column<int>(type: "int", nullable: false),
                    RecipeNumber = table.Column<int>(type: "int", nullable: false),
                    MixerTemperature = table.Column<float>(type: "real", nullable: false),
                    SetRevertMud = table.Column<float>(type: "real", nullable: false),
                    ActRevertMud = table.Column<float>(type: "real", nullable: false),
                    SetSandMud = table.Column<float>(type: "real", nullable: false),
                    ActSandMud = table.Column<float>(type: "real", nullable: false),
                    SetColdWater = table.Column<float>(type: "real", nullable: false),
                    ActColdWater = table.Column<float>(type: "real", nullable: false),
                    SetHotWater = table.Column<float>(type: "real", nullable: false),
                    ActHotWater = table.Column<float>(type: "real", nullable: false),
                    SetMixture1 = table.Column<float>(type: "real", nullable: false),
                    ActMixture1 = table.Column<float>(type: "real", nullable: false),
                    SetMixture2 = table.Column<float>(type: "real", nullable: false),
                    ActMixture2 = table.Column<float>(type: "real", nullable: false),
                    SetCement1 = table.Column<float>(type: "real", nullable: false),
                    ActCement1 = table.Column<float>(type: "real", nullable: false),
                    SetCement2 = table.Column<float>(type: "real", nullable: false),
                    ActCement2 = table.Column<float>(type: "real", nullable: false),
                    SetAluminium1 = table.Column<float>(type: "real", nullable: false),
                    ActAluminium1 = table.Column<float>(type: "real", nullable: false),
                    SetAluminium2 = table.Column<float>(type: "real", nullable: false),
                    ActAluminium2 = table.Column<float>(type: "real", nullable: false),
                    SandInMud = table.Column<float>(type: "real", nullable: false),
                    Normal = table.Column<bool>(type: "bit", nullable: false),
                    Undersized = table.Column<bool>(type: "bit", nullable: false),
                    Overground = table.Column<bool>(type: "bit", nullable: false),
                    Boiled = table.Column<bool>(type: "bit", nullable: false),
                    Other = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mixes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mixes");
        }
    }
}
