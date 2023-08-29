using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_mvc.Models
{
    public class AddCourseViewModel
    {
        [Required(ErrorMessage = "Course Name is required")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Faculty is required")]
        public string Faculty { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Credits is required")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Instructor is required")]
        public int SelectedInstructorId { get; set; }

        public List<SelectListItem> AvailableInstructors { get; set; }
    }
}
