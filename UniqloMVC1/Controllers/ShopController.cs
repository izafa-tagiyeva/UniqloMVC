using Microsoft.AspNetCore.Mvc;

namespace UniqloMVC1.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
