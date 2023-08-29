using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models
{
    public class GradeViewModel
    {
        public int EnrollmentID { get; set; }
        [Column("Grade")]
        public float AssignedGrade { get; set; }
        public DateTime Date { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
    }
}
