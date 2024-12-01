using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC1.DataAccess;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Category;

namespace UniqloMVC1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController(UniqloDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();


            Category category = new Category
            {
                Name = vm.Name
            };

            await _context.AddAsync(category);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return NotFound();

            CategoryUpdateVM vm = new();

            vm.Name = data.Name;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM vm)
        {

            if (!id.HasValue) return BadRequest();
            if (!ModelState.IsValid) return View();

            var data = await _context.Categories.FindAsync(id);

            if (data is null) return View();

            data.Name = vm.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var data = await _context.Categories.FindAsync(id);

            if (data is null) return View();


            _context.Categories.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
