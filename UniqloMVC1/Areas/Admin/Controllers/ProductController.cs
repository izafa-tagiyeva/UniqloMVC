using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC1.DataAccess;
using UniqloMVC1.FileExtensions;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Product;

namespace UniqloMVC1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController( UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(x => x.Category).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                {
                    ModelState.AddModelError("File", "File must be an image");
                }

                if (!vm.CoverFile.IsValidSize(24*1024))
                {
                    ModelState.AddModelError("File", "File size must be less than 24MB");
                }
            }

            if (!ModelState.IsValid) return View(vm);

            string newFileName = await vm.CoverFile.UploadAsync("wwwroot", "imgs", "products");

            Product product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                CostPrice = vm.CostPrice,
                SellPrice = vm.SellPrice,
                Quantity = vm.Quantity,
                Discount = vm.Discount,
                CoverImage = newFileName,
                CategoryId = vm.CategoryId
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
 
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////
      
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest();

            var data = await _context.Products
                .Where(x => x.Id == id)
                .Select(x => new ProductUpdateVM
                {   
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    CostPrice = x.CostPrice,
                    SellPrice = x.SellPrice, 
                    Discount = x.Discount,
                })
                .FirstOrDefaultAsync();

            if (data == null) return NotFound();
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<ActionResult> Update(int id, ProductUpdateVM vm)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            if (vm.CoverFile != null)
            {
                if (!vm.CoverFile.IsValidType("image"))
                {
                    ModelState.AddModelError("File", "File must be an image");
                }

                if (!vm.CoverFile.IsValidSize(24*1024))
                {
                    ModelState.AddModelError("File", "File size must be less than 24 MB");
                }

                string oldFilePath = Path.Combine(_env.WebRootPath, "imgs", "products", product.CoverImage);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                string newFileName = await vm.CoverFile.UploadAsync(_env.WebRootPath, "imgs", "products");
                product.CoverImage = newFileName;


            }

            

            if (!ModelState.IsValid) return View(vm);

            if (!await _context.Categories.AnyAsync(x => x.Id == vm.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category does not exist");
                return View(vm);
            }


            product.Name = vm.Name;
            product.Description = vm.Description;
            product.CostPrice = vm.CostPrice;
            product.SellPrice = vm.SellPrice;
            product.Discount = vm.Discount;
            product.CategoryId = vm.CategoryId;
            product.Quantity = vm.Quantity;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {


            Product product = await _context.Products.FindAsync(id);
            if (product is null) return NotFound();
            

            string filePath = Path.Combine(_env.WebRootPath, "imgs", "products", product.CoverImage);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _context.Products.FindAsync(id);

            if (product is null) return View();

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _context.Products.FindAsync(id);

            if (product is null) return View();

            product.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
