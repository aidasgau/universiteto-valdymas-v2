using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models
{
    public class EditGradeViewModel
    {
        public int EnrollmentID { get; set; }

        [Column("Grade")]
        [Required(ErrorMessage = "Assigned grade is required.")]
        public float AssignedGrade { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Selected enrollment ID is required.")]
        public int EnrollmentId { get; set; }

        public List<SelectListItem> AvailableEnrollments { get; set; }
    }
}
