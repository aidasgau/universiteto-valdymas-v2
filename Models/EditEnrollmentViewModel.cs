using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models
{
    public class EditEnrollmentViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Selected student ID is required.")]
        public int SelectedStudentId { get; set; }

        public List<SelectListItem> AvailableStudents { get; set; }

        [Required(ErrorMessage = "Selected course ID is required.")]
        public int SelectedCourseId { get; set; }

        public List<SelectListItem> AvailableCourses { get; set; }
    }
}
