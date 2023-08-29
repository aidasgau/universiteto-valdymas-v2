using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models.Domain
{
    public class Instructor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstructorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
    }
}
