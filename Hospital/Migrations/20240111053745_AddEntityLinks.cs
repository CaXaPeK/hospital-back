using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDiagnoses_Inspections_InspectionId",
                table: "InspectionDiagnoses");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Inspections",
                newName: "DoctorId");

            migrationBuilder.RenameColumn(
                name: "Speciality",
                table: "Doctors",
                newName: "SpecialityId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BaseInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "NextInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionId",
                table: "InspectionDiagnoses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Consultations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_BaseInspectionId",
                table: "Inspections",
                column: "BaseInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_NextInspectionId",
                table: "Inspections",
                column: "NextInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PreviousInspectionId",
                table: "Inspections",
                column: "PreviousInspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionDiagnoses_IcdDiagnosisId",
                table: "InspectionDiagnoses",
                column: "IcdDiagnosisId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecialityId",
                table: "Doctors",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_DoctorId",
                table: "Consultations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_SpecialityId",
                table: "Consultations",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments",
                column: "ParentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Doctors_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Doctors_DoctorId",
                table: "Consultations",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_Specialities_SpecialityId",
                table: "Consultations",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialities_SpecialityId",
                table: "Doctors",
                column: "SpecialityId",
                principalTable: "Specialities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDiagnoses_Diagnoses_IcdDiagnosisId",
                table: "InspectionDiagnoses",
                column: "IcdDiagnosisId",
                principalTable: "Diagnoses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDiagnoses_Inspections_InspectionId",
                table: "InspectionDiagnoses",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Doctors_DoctorId",
                table: "Inspections",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Inspections_BaseInspectionId",
                table: "Inspections",
                column: "BaseInspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Inspections_NextInspectionId",
                table: "Inspections",
                column: "NextInspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Inspections_PreviousInspectionId",
                table: "Inspections",
                column: "PreviousInspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Doctors_AuthorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Doctors_DoctorId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_Specialities_SpecialityId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialities_SpecialityId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDiagnoses_Diagnoses_IcdDiagnosisId",
                table: "InspectionDiagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDiagnoses_Inspections_InspectionId",
                table: "InspectionDiagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Doctors_DoctorId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Inspections_BaseInspectionId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Inspections_NextInspectionId",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Inspections_PreviousInspectionId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_BaseInspectionId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_DoctorId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_NextInspectionId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_PreviousInspectionId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_InspectionDiagnoses_IcdDiagnosisId",
                table: "InspectionDiagnoses");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecialityId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_DoctorId",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_SpecialityId",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "NextInspectionId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Consultations");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Inspections",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "SpecialityId",
                table: "Doctors",
                newName: "Speciality");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreviousInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "BaseInspectionId",
                table: "Inspections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InspectionId",
                table: "InspectionDiagnoses",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDiagnoses_Inspections_InspectionId",
                table: "InspectionDiagnoses",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id");
        }
    }
}
