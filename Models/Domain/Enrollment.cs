using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models.Domain
{
    public class Enrollment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }
    }
}
