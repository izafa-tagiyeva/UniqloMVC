using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniqloMVC1.DataAccess;
using UniqloMVC1.ViewModels.Slider;
using UniqloMVC1.ViewModels.Product;
using UniqloMVC1.ViewModels.Commons;


namespace UniqloMVC1.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();

            vm.Sliders = await _context.Sliders
                .Where(x => !x.IsDeleted)
                .Select(x => new SliderItemVM
                {
                    ImageUrl = x.ImageUrl,
                    Link = x.Link,
                    Title = x.Title,
                    Subtitle = x.Subtitle
                }).ToListAsync();

            vm.Products = await _context.Products
                .Where(x => !x.IsDeleted)
                .Select(x => new ProductItemVM
                {
                    Discount = x.Discount,
                    Id = x.Id,
                    ImageUrl = x.CoverImage,
                    IsInStock = x.Quantity > 0,
                    Name = x.Name,
                    Price = x.SellPrice
                }).ToListAsync();

            return View(vm);
        }

        public IActionResult About()
        {
            return View();
        }
       
        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> AccesDenied()
        {
            return View();
        }



    }
}
