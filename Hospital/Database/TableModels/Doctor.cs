﻿using Hospital.Models.General;
using Hospital.Validation;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Database.TableModels
{
    public class Doctor
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression("[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\\.[a-zA-Z0-9_-]+",
        ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [BirthDate]
        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
        ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [Required]
        public Guid SpecialityId { get; set; }

        public Speciality Speciality { get; set; }

        public List<Inspection> Inspections { get; set; }

        public List<Consultation> Consultations { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
