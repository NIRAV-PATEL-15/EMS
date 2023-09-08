using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    public class Employee : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
