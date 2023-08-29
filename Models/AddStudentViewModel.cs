using System;
using System.ComponentModel.DataAnnotations;

namespace project_mvc.Models
{
    public class AddStudentViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Program is required.")]
        public string Program { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "GPA is required.")]
        [Range(0, 4.0, ErrorMessage = "GPA must be between 0 and 4.")]
        public float GPA { get; set; }
    }
}
