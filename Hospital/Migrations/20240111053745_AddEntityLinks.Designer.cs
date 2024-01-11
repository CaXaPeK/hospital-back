﻿// <auto-generated />
using System;
using Hospital.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hospital.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240111053745_AddEntityLinks")]
    partial class AddEntityLinks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Hospital.Database.TableModels.BannedToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("BannedTokens");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConsultationId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ConsultationId");

                    b.HasIndex("ParentId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Consultation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("InspectionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("InspectionId");

                    b.HasIndex("SpecialityId");

                    b.ToTable("Consultations");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Diagnosis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Actual")
                        .HasColumnType("boolean");

                    b.Property<int?>("AddlCode")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<string>("MkbCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MkbName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<string>("RecCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RootCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Diagnoses");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Doctor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SpecialityId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Inspection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Anamnesis")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<Guid?>("BaseInspectionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Complaints")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<int>("Conclusion")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeathDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("NextInspectionId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("NextVisitDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PreviousInspectionId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Treatment")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.HasKey("Id");

                    b.HasIndex("BaseInspectionId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("NextInspectionId");

                    b.HasIndex("PatientId");

                    b.HasIndex("PreviousInspectionId");

                    b.ToTable("Inspections");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.InspectionDiagnosis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<Guid>("IcdDiagnosisId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("InspectionId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IcdDiagnosisId");

                    b.HasIndex("InspectionId");

                    b.ToTable("InspectionDiagnoses");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Patient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Speciality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Comment", b =>
                {
                    b.HasOne("Hospital.Database.TableModels.Doctor", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Consultation", "Consultation")
                        .WithMany("Comments")
                        .HasForeignKey("ConsultationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Comment", "Parent")
                        .WithMany("ChildComments")
                        .HasForeignKey("ParentId");

                    b.Navigation("Author");

                    b.Navigation("Consultation");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Consultation", b =>
                {
                    b.HasOne("Hospital.Database.TableModels.Doctor", null)
                        .WithMany("Consultations")
                        .HasForeignKey("DoctorId");

                    b.HasOne("Hospital.Database.TableModels.Inspection", "Inspection")
                        .WithMany("Consultations")
                        .HasForeignKey("InspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inspection");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Doctor", b =>
                {
                    b.HasOne("Hospital.Database.TableModels.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Inspection", b =>
                {
                    b.HasOne("Hospital.Database.TableModels.Inspection", "BaseInspection")
                        .WithMany()
                        .HasForeignKey("BaseInspectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Hospital.Database.TableModels.Doctor", "Doctor")
                        .WithMany("Inspections")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Inspection", "NextInspection")
                        .WithMany()
                        .HasForeignKey("NextInspectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Hospital.Database.TableModels.Patient", "Patient")
                        .WithMany("Inspections")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Inspection", "PreviousInspection")
                        .WithMany()
                        .HasForeignKey("PreviousInspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BaseInspection");

                    b.Navigation("Doctor");

                    b.Navigation("NextInspection");

                    b.Navigation("Patient");

                    b.Navigation("PreviousInspection");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.InspectionDiagnosis", b =>
                {
                    b.HasOne("Hospital.Database.TableModels.Diagnosis", "IcdDiagnosis")
                        .WithMany()
                        .HasForeignKey("IcdDiagnosisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hospital.Database.TableModels.Inspection", "Inspection")
                        .WithMany("Diagnoses")
                        .HasForeignKey("InspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IcdDiagnosis");

                    b.Navigation("Inspection");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Comment", b =>
                {
                    b.Navigation("ChildComments");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Consultation", b =>
                {
                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Doctor", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Consultations");

                    b.Navigation("Inspections");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Inspection", b =>
                {
                    b.Navigation("Consultations");

                    b.Navigation("Diagnoses");
                });

            modelBuilder.Entity("Hospital.Database.TableModels.Patient", b =>
                {
                    b.Navigation("Inspections");
                });
#pragma warning restore 612, 618
        }
    }
}