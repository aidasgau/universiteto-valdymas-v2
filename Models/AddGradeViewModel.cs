using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models
{
    public class AddGradeViewModel
    {
        [Required(ErrorMessage = "Enrollment ID is required.")]
        public int EnrollmentID { get; set; }

        [Required(ErrorMessage = "Assigned grade is required.")]
        [Column("Grade")]
        public float AssignedGrade { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please select an enrollment.")]
        public int SelectedEnrollmentId { get; set; }

        public List<SelectListItem> AvailableEnrollments { get; set; }
    }
}
