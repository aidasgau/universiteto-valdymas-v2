using Microsoft.AspNetCore.Mvc;

namespace project_mvc.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        protected string GetSortOrder(string columnName)
        {
            var currentSortOrder = ViewBag.CurrentSortOrder;

            if (currentSortOrder != null && currentSortOrder.StartsWith(columnName))
            {
                return currentSortOrder.EndsWith("_asc") ? $"{columnName}_desc" : $"{columnName}_asc";
            }

            return $"{columnName}_asc";
        }

        protected string GetSortIcon(string columnName)
        {
            var currentSortOrder = ViewBag.CurrentSortOrder;

            if (currentSortOrder != null && currentSortOrder.StartsWith(columnName))
            {
                return currentSortOrder.EndsWith("_asc") ? "fa-sort-up" : "fa-sort-down";
            }

            return "";
        }
    }
}
