using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models.Domain
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public int InstructorID { get; set; }
        public string Faculty { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Credits { get; set; }
    }
}
