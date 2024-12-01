using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniqloMVC1.DataAccess;
using UniqloMVC1.ViewModels.Slider;
using UniqloMVC1.ViewModels.Product;


namespace UniqloMVC1.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var datas = await _context.Sliders.Where(x => !x.IsDeleted).Select(x => new SliderItemVM
            {

                Title = x.Title,
                Subtitle = x.Subtitle,
                Link = x.Link,
                ImageUrl = x.ImageUrl
            }).ToListAsync();
            return View(datas);
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
