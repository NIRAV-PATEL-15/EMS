using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
