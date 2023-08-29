using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Pdf.Annotations;
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
    public class EnrollmentsController : BaseController
    {
        private readonly MySqlDbContext mySqlDbContext;

        public EnrollmentsController(MySqlDbContext mySqlDbContext)
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
                var enrollmentsQuery = GetEnrollmentsQuery();

                if (!string.IsNullOrEmpty(search))
                {
                    enrollmentsQuery = ApplySearch(enrollmentsQuery, search);
                }

                if (!string.IsNullOrEmpty(sortOrder))
                {
                    enrollmentsQuery = ApplySorting(enrollmentsQuery, sortOrder);
                    ViewBag.CurrentSortOrder = sortOrder;
                }

                var enrollments = await enrollmentsQuery.ToListAsync();

                return View(enrollments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ErrorUtils.GetErrorMessage(ex, "enrollments");
                return RedirectToAction("ErrorProduction", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var students = await GetAvailableStudents();
            students.Insert(0, new SelectListItem { Value = "", Text = "Select a Student" });

            var courses = await GetAvailableCourses();
            courses.Insert(0, new SelectListItem { Value = "", Text = "Select a Course" });

            var viewModel = new AddEnrollmentViewModel
            {
                AvailableStudents = students,
                AvailableCourses = courses
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEnrollmentViewModel addEnrollmentRequest)
        {
            var enrollment = new Enrollment()
            {
                StudentID = addEnrollmentRequest.SelectedStudentId,
                CourseID = addEnrollmentRequest.SelectedCourseId,
                Status = addEnrollmentRequest.Status,
                Date = addEnrollmentRequest.Date,
            };
            await mySqlDbContext.Enrollments.AddAsync(enrollment);
            await mySqlDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var enrollment = await mySqlDbContext.Enrollments.FindAsync(Id);

            if (enrollment == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new EditEnrollmentViewModel()
            {
                EnrollmentID = enrollment.EnrollmentID,
                Status = enrollment.Status,
                Date = enrollment.Date,
                SelectedStudentId = enrollment.StudentID,
                AvailableStudents = await GetAvailableStudents(),
                SelectedCourseId = enrollment.CourseID,
                AvailableCourses = await GetAvailableCourses()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditEnrollmentViewModel model)
        {
            var enrollment = await mySqlDbContext.Enrollments.FindAsync(model.EnrollmentID);

            if (enrollment == null)
            {
                return RedirectToAction("Index");
            }
            enrollment.Status = model.Status;
            enrollment.Date = model.Date;
            enrollment.StudentID = model.SelectedStudentId;
            enrollment.CourseID = model.SelectedCourseId;

            await mySqlDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetAvailableStudents()
        {
            return await mySqlDbContext.Students
                .Select(i => new SelectListItem
                {
                    Value = i.StudentID.ToString(),
                    Text = i.FirstName + " " + i.LastName
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetAvailableCourses()
        {
            return await mySqlDbContext.Courses
              .Select(i => new SelectListItem
              {
                  Value = i.CourseID.ToString(),
                  Text = i.CourseName
              })
              .ToListAsync();
        }

        [HttpGet]
        public IActionResult GeneratePdf()
        {
            var enrollmentsQuery = GetEnrollmentsQuery();
            var enrollments = enrollmentsQuery.ToList();

            return new ViewAsPdf("GeneratePdf", enrollments)
            {
                FileName = "Enrollments.pdf"
            };
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var enrollment = await mySqlDbContext.Enrollments.FindAsync(Id);
            if (enrollment != null)
            {
                mySqlDbContext.Enrollments.Remove(enrollment);
                await mySqlDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private IQueryable<EnrollmentViewModel> ApplySearch(IQueryable<EnrollmentViewModel> enrollmentsQuery, string search)
        {
            enrollmentsQuery = enrollmentsQuery.Where(g => g.StudentName.Contains(search) || g.CourseName.Contains(search));
            return enrollmentsQuery;
        }

        private IQueryable<EnrollmentViewModel> ApplySorting(IQueryable<EnrollmentViewModel> enrollmentsQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "student_asc":
                    return enrollmentsQuery.OrderBy(g => g.StudentName);
                case "course_asc":
                    return enrollmentsQuery.OrderBy(g => g.CourseName);
                case "status_asc":
                    return enrollmentsQuery.OrderBy(g => g.Status);
                case "date_asc":
                    return enrollmentsQuery.OrderBy(g => g.Date);
                case "student_desc":
                    return enrollmentsQuery.OrderByDescending(g => g.StudentName);
                case "course_desc":
                    return enrollmentsQuery.OrderByDescending(g => g.CourseName);
                case "status_desc":
                    return enrollmentsQuery.OrderByDescending(g => g.Status);
                case "date_desc":
                    return enrollmentsQuery.OrderByDescending(g => g.Date);
                default:
                    return enrollmentsQuery.OrderBy(g => g.EnrollmentID);
            }
        }

        private IQueryable<EnrollmentViewModel> GetEnrollmentsQuery()
        {
            var enrollmentsQuery = mySqlDbContext.Enrollments
                    .Join(mySqlDbContext.Students, student => student.StudentID, enrollment => enrollment.StudentID, (enrollment, student) => new { Enrollment = enrollment, Student = student })
                    .Join(mySqlDbContext.Courses, joinResult => joinResult.Enrollment.CourseID, course => course.CourseID, (joinResult, course) => new EnrollmentViewModel
                    {
                        EnrollmentID = joinResult.Enrollment.EnrollmentID,
                        StudentID = joinResult.Enrollment.StudentID,
                        CourseID = joinResult.Enrollment.CourseID,
                        Status = joinResult.Enrollment.Status,
                        Date = joinResult.Enrollment.Date,
                        StudentName = joinResult.Student.FirstName + " " + joinResult.Student.LastName,
                        CourseName = course.CourseName
                    });

            return enrollmentsQuery;
        }
    }
}
