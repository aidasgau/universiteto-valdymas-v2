using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models.Domain
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Program { get; set; }
        public int Year { get; set; }
        public float GPA { get; set; }
    }
}
