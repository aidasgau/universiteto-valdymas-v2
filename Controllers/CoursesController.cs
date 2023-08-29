using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project_mvc.Data;
using project_mvc.Models;
using project_mvc.Models.Domain;
using project_mvc.Utilities;
using Rotativa.AspNetCore;


namespace project_mvc.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly MySqlDbContext mySqlDbContext;

        public CoursesController(MySqlDbContext mySqlDbContext)
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
                IQueryable<Course> coursesQuery = mySqlDbContext.Courses.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    coursesQuery = coursesQuery.Where(c => c.CourseName.Contains(search));
                }

                if (!string.IsNullOrEmpty(sortOrder))
                {
                    coursesQuery = ApplySorting(coursesQuery, sortOrder);
                    ViewBag.CurrentSortOrder = sortOrder;
                }

                var courses = await coursesQuery.ToListAsync();
                return View(courses);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ErrorUtils.GetErrorMessage(ex, "courses");
                return RedirectToAction("ErrorProduction", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var instructors = await GetAvailableInstructors();
            instructors.Insert(0, new SelectListItem { Value = "", Text = "Select an Instructor" });

            var viewModel = new AddCourseViewModel
            {
                AvailableInstructors = instructors
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddCourseViewModel addCourseRequest)
        {
            var course = new Course()
            {
                CourseName = addCourseRequest.CourseName,
                InstructorID = addCourseRequest.SelectedInstructorId,
                Faculty = addCourseRequest.Faculty,
                StartDate = addCourseRequest.StartDate,
                EndDate = addCourseRequest.EndDate,
                Credits = addCourseRequest.Credits,
            };
            await mySqlDbContext.Courses.AddAsync(course);
            await mySqlDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var course = await mySqlDbContext.Courses.FirstOrDefaultAsync(x => x.CourseID == Id);
            var instructors = await GetAvailableInstructors();
            instructors.Insert(0, new SelectListItem { Value = "", Text = "Select an Instructor" });

            if (course != null)
            {
                var viewModel = new EditCourseViewModel()
                {
                    CourseID = course.CourseID,
                    CourseName = course.CourseName,
                    SelectedInstructorId = course.InstructorID,
                    Faculty = course.Faculty,
                    StartDate = course.StartDate,
                    EndDate = course.EndDate,
                    Credits = course.Credits,
                    AvailableInstructors = instructors,
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCourseViewModel model)
        {
            var course = await mySqlDbContext.Courses.FindAsync(model.CourseID);

            if (course != null)
            {
                course.CourseName = model.CourseName;
                course.InstructorID = model.SelectedInstructorId;
                course.Faculty = model.Faculty;
                course.StartDate = model.StartDate;
                course.EndDate = model.EndDate;
                course.Credits = model.Credits;

                await mySqlDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private IQueryable<Course> ApplySorting(IQueryable<Course> coursesQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "course_asc":
                    return coursesQuery.OrderBy(g => g.CourseName);
                case "instructor_asc":
                    return coursesQuery.OrderBy(g => g.InstructorID);
                case "faculty_asc":
                    return coursesQuery.OrderBy(g => g.Faculty);
                case "startdate_asc":
                    return coursesQuery.OrderBy(g => g.StartDate);
                case "enddate_asc":
                    return coursesQuery.OrderBy(g => g.EndDate);
                case "credits_asc":
                    return coursesQuery.OrderBy(g => g.Credits);
                case "course_desc":
                    return coursesQuery.OrderByDescending(g => g.CourseName);
                case "instructor_desc":
                    return coursesQuery.OrderByDescending(g => g.InstructorID);
                case "faculty_desc":
                    return coursesQuery.OrderByDescending(g => g.Faculty);
                case "startdate_desc":
                    return coursesQuery.OrderByDescending(g => g.StartDate);
                case "enddate_desc":
                    return coursesQuery.OrderByDescending(g => g.EndDate);
                case "credits_desc":
                    return coursesQuery.OrderByDescending(g => g.Credits);
                default:
                    return coursesQuery.OrderBy(g => g.CourseID);
            }
        }

        [HttpGet]
        public IActionResult GeneratePdf()
        {
            IQueryable<Course> coursesQuery = mySqlDbContext.Courses.AsQueryable();
            var courses = coursesQuery.ToListAsync();

            return new ViewAsPdf("GeneratePdf", courses)
            {
                FileName = "Courses.pdf"
            };
        }
   
        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var course = await mySqlDbContext.Courses.FindAsync(Id);
            if (course != null)
            {
                mySqlDbContext.Courses.Remove(course);
                await mySqlDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetAvailableInstructors()
        {
            return await mySqlDbContext.Instructors
                .Select(i => new SelectListItem
                {
                    Value = i.InstructorID.ToString(),
                    Text = i.FirstName + " " + i.LastName
                })
                .ToListAsync();
        }
    }
}
