using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Actual = table.Column<bool>(type: "boolean", nullable: false),
                    AddlCode = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    MkbCode = table.Column<string>(type: "text", nullable: false),
                    MkbName = table.Column<string>(type: "text", nullable: false),
                    RecCode = table.Column<string>(type: "text", nullable: false),
                    RootCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_Id",
                table: "Diagnoses",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");
        }
    }
}
