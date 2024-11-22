using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace UniqloMVC1.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
