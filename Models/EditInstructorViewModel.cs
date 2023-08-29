using System.ComponentModel.DataAnnotations;

namespace project_mvc.Models
{
    public class EditInstructorViewModel
    {
        [Required(ErrorMessage = "Instructor ID is required.")]
        public int InstructorID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Faculty is required.")]
        public string Faculty { get; set; }
    }
}
