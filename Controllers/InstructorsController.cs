using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using project_mvc.Data;
using project_mvc.Models;
using project_mvc.Models.Domain;
using project_mvc.Utilities;
using Rotativa.AspNetCore;
using System.Data.Common;

namespace project_mvc.Controllers
{
    public class InstructorsController : BaseController
    {
        private readonly MySqlDbContext mySqlDbContext;

        public InstructorsController(MySqlDbContext mySqlDbContext)
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
                IQueryable<Instructor> instructorsQuery = mySqlDbContext.Instructors;

                if (!string.IsNullOrEmpty(search))
                {
                    instructorsQuery = instructorsQuery.Where(i =>
                        EF.Functions.Like(i.FirstName, $"%{search}%") ||
                        EF.Functions.Like(i.LastName, $"%{search}%") ||
                        EF.Functions.Like(i.FirstName + " " + i.LastName, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(sortOrder))
                {
                    instructorsQuery = ApplySorting(instructorsQuery, sortOrder);
                    ViewBag.CurrentSortOrder = sortOrder;
                }

                var instructors = await instructorsQuery.ToListAsync();
                return View(instructors);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ErrorUtils.GetErrorMessage(ex, "instructors");
                return RedirectToAction("ErrorProduction", "Home");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInstructorViewModel addInstructorRequest)
        {
            var instructor = new Instructor()
            {
                FirstName = addInstructorRequest.FirstName,
                LastName = addInstructorRequest.LastName,
                Email = addInstructorRequest.Email,
                Faculty = addInstructorRequest.Faculty,
            };
            await mySqlDbContext.Instructors.AddAsync(instructor);
            await mySqlDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var instructor = await mySqlDbContext.Instructors.FirstOrDefaultAsync(x => x.InstructorID == Id);

            if (instructor != null)
            {
                var viewModel = new EditInstructorViewModel()
                {
                    InstructorID = instructor.InstructorID,
                    FirstName = instructor.FirstName,
                    LastName = instructor.LastName,
                    Email = instructor.Email,
                    Faculty = instructor.Faculty,
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditInstructorViewModel model)
        {
            var instructor = await mySqlDbContext.Instructors.FindAsync(model.InstructorID);

            if (instructor != null)
            {
                instructor.FirstName = model.FirstName;
                instructor.LastName = model.LastName;
                instructor.Email = model.Email;
                instructor.Faculty = model.Faculty;

                await mySqlDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        private IQueryable<Instructor> ApplySorting(IQueryable<Instructor> instructorsQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "firstname_asc":
                    return instructorsQuery.OrderBy(g => g.FirstName);
                case "lastname_asc":
                    return instructorsQuery.OrderBy(g => g.LastName);
                case "email_asc":
                    return instructorsQuery.OrderBy(g => g.Email);
                case "faculty_asc":
                    return instructorsQuery.OrderBy(g => g.Faculty);   
                case "firstname_desc":
                    return instructorsQuery.OrderByDescending(g => g.FirstName);
                case "lastname_desc":
                    return instructorsQuery.OrderByDescending(g => g.LastName);
                case "email_desc":
                    return instructorsQuery.OrderByDescending(g => g.Email);
                case "faculty_desc":
                    return instructorsQuery.OrderByDescending(g => g.Faculty);
                default:
                    return instructorsQuery.OrderBy(g => g.InstructorID);
            }
        }

        [HttpGet]
        public IActionResult GeneratePdf()
        {
            IQueryable<Instructor> instructorsQuery = mySqlDbContext.Instructors;
            var instructors = instructorsQuery.ToListAsync();

            return new ViewAsPdf("GeneratePdf", instructors)
            {
                FileName = "Instructors.pdf"
            };
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var instructor = await mySqlDbContext.Instructors.FindAsync(Id);
            if (instructor != null)
            {
                mySqlDbContext.Instructors.Remove(instructor);
                await mySqlDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
