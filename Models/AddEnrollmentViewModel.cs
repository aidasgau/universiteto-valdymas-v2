using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_mvc.Models
{
    public class AddEnrollmentViewModel
    {
        [Required(ErrorMessage = "Status is required.")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please select a student.")]
        public int SelectedStudentId { get; set; }

        public List<SelectListItem> AvailableStudents { get; set; }

        [Required(ErrorMessage = "Please select a course.")]
        public int SelectedCourseId { get; set; }

        public List<SelectListItem> AvailableCourses { get; set; }
    }
}
