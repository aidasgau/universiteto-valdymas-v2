using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models
{
    public class EditCourseViewModel
    {
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        public string CourseName { get; set; }

       // [Required(ErrorMessage = "Instructor ID is required.")]
        //public int InstructorID { get; set; }

        [Required(ErrorMessage = "Faculty is required.")]
        public string Faculty { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Credits is required.")]
        public int Credits { get; set; }

        public int SelectedInstructorId { get; set; }

        public List<SelectListItem> AvailableInstructors { get; set; }
    }
}
