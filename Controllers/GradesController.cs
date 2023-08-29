using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_mvc.Data;
using project_mvc.Models;
using project_mvc.Models.Domain;
using project_mvc.Utilities;
using Rotativa.AspNetCore;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace project_mvc.Controllers
{
    public class GradesController : BaseController
    {
        private readonly MySqlDbContext mySqlDbContext;

        public GradesController(MySqlDbContext mySqlDbContext)
        {
            this.mySqlDbContext = mySqlDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search, string sortOrder)
        {
            try
            {
                ViewBag.GetSortOrder = new Func<string, string>(GetSortOrder);
                ViewBag.GetSortIcon = new Func<string, string>(GetSortIcon);
                var gradesQuery = GetGradesQuery();

                if (!string.IsNullOrEmpty(search))
                {
                    gradesQuery = ApplySearch(gradesQuery, search);
                }

                if (!string.IsNullOrEmpty(sortOrder))
                {
                    gradesQuery = ApplySorting(gradesQuery, sortOrder);
                    ViewBag.CurrentSortOrder = sortOrder;
                }

                var grades = await gradesQuery.ToListAsync();

                return View(grades);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ErrorUtils.GetErrorMessage(ex, "grades");
                return RedirectToAction("ErrorProduction", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var enrollments = await GetAvailableEnrollments();
            enrollments.Insert(0, new SelectListItem { Value = "", Text = "Select an Enrollment" });

            var viewModel = new AddGradeViewModel
            {
                AvailableEnrollments = enrollments
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddGradeViewModel addGradeRequest)
        {
            var grade = new Grade()
            {
                EnrollmentID = addGradeRequest.SelectedEnrollmentId,
                AssignedGrade = addGradeRequest.AssignedGrade,
                Date = addGradeRequest.Date,
            };
            await mySqlDbContext.Grades.AddAsync(grade);
            await mySqlDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int enrollmentId)
        {
            var grade = await mySqlDbContext.Grades.FirstOrDefaultAsync(g => g.EnrollmentID == enrollmentId);

            if (grade != null)
            {
                var viewModel = new EditGradeViewModel()
                {
                    EnrollmentID = grade.EnrollmentID,
                    AssignedGrade = grade.AssignedGrade,
                    Date = grade.Date,
                    AvailableEnrollments = await GetAvailableEnrollments()
                };

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditGradeViewModel model)
        {
            var grade = await mySqlDbContext.Grades.FirstOrDefaultAsync(g => g.EnrollmentID == model.EnrollmentID);

            if (grade != null)
            {
                grade.AssignedGrade = model.AssignedGrade;
                grade.Date = model.Date;

                await mySqlDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetAvailableEnrollments()
        {
            var enrollments = await mySqlDbContext.Enrollments.ToListAsync();

            var studentIds = enrollments.Select(e => e.StudentID).Distinct().ToList();
            var students = await mySqlDbContext.Students
                .Where(s => studentIds.Contains(s.StudentID))
                .ToListAsync();

            return enrollments.Select(e => new SelectListItem
            {
                Value = e.EnrollmentID.ToString(),
                Text = GetStudentName(students, e.StudentID)
            }).ToList();
        }

        private string GetStudentName(List<Student> students, int studentId)
        {
            var student = students.FirstOrDefault(s => s.StudentID == studentId);
            return student != null ? $"{student.FirstName} {student.LastName}" : string.Empty;
        }


        [HttpGet]
        public IActionResult GeneratePdf()
        {
            var gradesQuery = GetGradesQuery();
            var grades = gradesQuery.ToList();

            return new ViewAsPdf("GeneratePdf", grades)
            {
                FileName = "Grades.pdf"
            };
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int enrollmentId)
        {
            var grade = await mySqlDbContext.Grades.FirstOrDefaultAsync(g => g.EnrollmentID == enrollmentId);
            if (grade != null)
            {
                mySqlDbContext.Grades.Remove(grade);
                await mySqlDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private IQueryable<GradeViewModel> ApplySorting(IQueryable<GradeViewModel> gradesQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "student_asc":
                    return gradesQuery.OrderBy(g => g.StudentName);
                case "course_asc":
                    return gradesQuery.OrderBy(g => g.CourseName);
                case "grade_asc":
                    return gradesQuery.OrderBy(g => g.AssignedGrade);
                case "date_asc":
                    return gradesQuery.OrderBy(g => g.Date);
                case "student_desc":
                    return gradesQuery.OrderByDescending(g => g.StudentName);
                case "course_desc":
                    return gradesQuery.OrderByDescending(g => g.CourseName);
                case "grade_desc":
                    return gradesQuery.OrderByDescending(g => g.AssignedGrade);
                case "date_desc":
                    return gradesQuery.OrderByDescending(g => g.Date);
                default:
                    return gradesQuery.OrderBy(g => g.EnrollmentID);
            }
        }

        private IQueryable<GradeViewModel> ApplySearch(IQueryable<GradeViewModel> gradesQuery, string search)
        {
            gradesQuery = gradesQuery.Where(g => g.StudentName.Contains(search) || g.CourseName.Contains(search));
            return gradesQuery;
        }

        private IQueryable<GradeViewModel> GetGradesQuery()
        {
            var gradesQuery = from grade in mySqlDbContext.Grades
                              join enrollment in mySqlDbContext.Enrollments on grade.EnrollmentID equals enrollment.EnrollmentID
                              join student in mySqlDbContext.Students on enrollment.StudentID equals student.StudentID
                              join course in mySqlDbContext.Courses on enrollment.CourseID equals course.CourseID
                              select new GradeViewModel
                              {
                                  EnrollmentID = grade.EnrollmentID,
                                  AssignedGrade = grade.AssignedGrade,
                                  Date = grade.Date,
                                  StudentID = student.StudentID,
                                  CourseID = course.CourseID,
                                  StudentName = student.FirstName + " " + student.LastName,
                                  CourseName = course.CourseName
                              };

            return gradesQuery;

        }
    }
}
