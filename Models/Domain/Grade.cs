using System.ComponentModel.DataAnnotations.Schema;

namespace project_mvc.Models.Domain
{
    public class Grade
    {
        public int EnrollmentID { get; set; }
        [Column("Grade")]
        public float AssignedGrade { get; set; }
        public DateTime Date { get; set; }
    }
}
