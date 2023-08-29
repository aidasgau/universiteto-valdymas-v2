using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_mvc.Data;
using project_mvc.Models;
using project_mvc.Models.Domain;
using project_mvc.Utilities;
using Rotativa.AspNetCore;

namespace project_mvc.Controllers
{
    public class StudentsController : BaseController
    {
        private readonly MySqlDbContext mySqlDbContext;

        public StudentsController(MySqlDbContext mySqlDbContext)
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
                IQueryable<Student> studentsQuery = mySqlDbContext.Students;

                if (!string.IsNullOrEmpty(search))
                {
                    studentsQuery = studentsQuery.Where(i =>
                        EF.Functions.Like(i.FirstName, $"%{search}%") ||
                        EF.Functions.Like(i.LastName, $"%{search}%") ||
                        EF.Functions.Like(i.FirstName + " " + i.LastName, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(sortOrder))
                {
                    studentsQuery = ApplySorting(studentsQuery, sortOrder);
                    ViewBag.CurrentSortOrder = sortOrder;
                }

                var students = await studentsQuery.ToListAsync();
                return View(students);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ErrorUtils.GetErrorMessage(ex, "students");
                return RedirectToAction("ErrorProduction", "Home");
            }
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel addStudentRequest) 
        {
            var student = new Student()
            {
                FirstName = addStudentRequest.FirstName,
                LastName = addStudentRequest.LastName,
                Email = addStudentRequest.Email,
                DateOfBirth = addStudentRequest.DateOfBirth,
                Address = addStudentRequest.Address,
                Program = addStudentRequest.Program,
                Year = addStudentRequest.Year,
                GPA = addStudentRequest.GPA
            };
            await mySqlDbContext.Students.AddAsync(student);
            await mySqlDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var student = await mySqlDbContext.Students.FirstOrDefaultAsync(x => x.StudentID == Id);

            if (student != null)
            {
                var viewModel = new EditStudentViewModel()
                {
                    StudentID = student.StudentID,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    Address = student.Address,
                    Program = student.Program,
                    Year = student.Year,
                    GPA = student.GPA
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditStudentViewModel model)
        {
            var student = await mySqlDbContext.Students.FindAsync(model.StudentID);

            if (student != null)
            {
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.Email = model.Email;
                student.DateOfBirth = model.DateOfBirth;
                student.Address = model.Address;
                student.Program = model.Program;
                student.Year = model.Year;
                student.GPA = model.GPA;

                await mySqlDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private IQueryable<Student> ApplySorting(IQueryable<Student> studentsQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "firstname_asc":
                    return studentsQuery.OrderBy(g => g.FirstName);
                case "lastname_asc":
                    return studentsQuery.OrderBy(g => g.LastName);
                case "email_asc":
                    return studentsQuery.OrderBy(g => g.Email);
                case "dateofbirth_asc":
                    return studentsQuery.OrderBy(g => g.DateOfBirth);
                case "address_asc":
                    return studentsQuery.OrderBy(g => g.Address);
                case "program_asc":
                    return studentsQuery.OrderBy(g => g.Program);
                case "year_asc":
                    return studentsQuery.OrderBy(g => g.Year);
                case "gpa_asc":
                    return studentsQuery.OrderBy(g => g.GPA);
                case "firstname_desc":
                    return studentsQuery.OrderByDescending(g => g.FirstName);
                case "lastname_desc":
                    return studentsQuery.OrderByDescending(g => g.LastName);
                case "email_desc":
                    return studentsQuery.OrderByDescending(g => g.Email);
                case "dateofbirth_desc":
                    return studentsQuery.OrderByDescending(g => g.DateOfBirth);
                case "address_desc":
                    return studentsQuery.OrderByDescending(g => g.Year);
                case "program_desc":
                    return studentsQuery.OrderByDescending(g => g.Program);
                case "year_desc":
                    return studentsQuery.OrderByDescending(g => g.Year);
                case "gpa_desc":
                    return studentsQuery.OrderByDescending(g => g.GPA);
                default:
                    return studentsQuery.OrderBy(g => g.StudentID);
            }
        }

        [HttpGet]
        public IActionResult GeneratePdf()
        {
            IQueryable<Student> studentsQuery = mySqlDbContext.Students;
            var students = studentsQuery.ToListAsync();

            return new ViewAsPdf("GeneratePdf", students)
            {
                FileName = "Students.pdf"
            };
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var student = await mySqlDbContext.Students.FindAsync(Id);
            if (student != null)
            {
                mySqlDbContext.Students.Remove(student);
                await mySqlDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
