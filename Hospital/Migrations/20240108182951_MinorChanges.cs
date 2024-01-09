using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Migrations
{
    /// <inheritdoc />
    public partial class MinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Specialities_SpecialityId",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_SpecialityId",
                table: "Consultations");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Inspections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BaseInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "BaseInspectionId",
                table: "Inspections");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_SpecialityId",
                table: "Consultations",
                column: "SpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Specialities_SpecialityId",
                table: "Consultations",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
