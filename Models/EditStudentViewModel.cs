using System;
using System.ComponentModel.DataAnnotations;

namespace project_mvc.Models
{
    public class EditStudentViewModel
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Program is required.")]
        public string Program { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "GPA is required.")]
        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0.")]
        public float GPA { get; set; }
    }
}
