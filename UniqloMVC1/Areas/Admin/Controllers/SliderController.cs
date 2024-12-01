using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloMVC1.DataAccess;
using UniqloMVC1.FileExtensions;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Slider;

namespace UniqloMVC1.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController(UniqloDbContext _context,IWebHostEnvironment _env) : Controller
    {


        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (vm.File != null)
            {
                if (!vm.File.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "File type must be image"); }
                if (vm.File.Length > 600 * 1024 * 1024) // add * 1024
                    ModelState.AddModelError("File", "File length must be less than 600mb");
            }
            if (!ModelState.IsValid) return View();


            
            string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName))) // Investigate!!!
            {
                await vm.File.CopyToAsync(stream);
            }

            Slider slider = new Slider
            {
                ImageUrl = newFileName,
                Link = vm.Link,
                Subtitle = vm.Subtitle,
                Title = vm.Title,
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }




        ///////////////////////////////////


        public async Task<IActionResult> Update(int id)
        {
            Slider? data = await _context.Sliders.FindAsync(id);
            if (data is null) return NotFound();
            SliderUpdateVM vm = new SliderUpdateVM();
            
            vm.Title = data.Title;
            vm.Subtitle = data.Subtitle;
            vm.Link = data.Link;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, SliderUpdateVM vm)
        {
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();
            if (!ModelState.IsValid) return View();

            if (vm.File != null)
            {
                if (!vm.File.ContentType.StartsWith("image"))
                {
                    ModelState.AddModelError("File", "File type must be image");
                    return View();
                }

                if (vm.File.Length > 600 * 1024 * 1024)
                {
                    ModelState.AddModelError("File", "File size must be less than 600MB");
                    return View();
                }

                string oldFilePath = Path.Combine(_env.WebRootPath, "imgs", "sliders", data.ImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                string newFileName = await vm.File.UploadAsync(_env.WebRootPath, "imgs", "sliders");
                data.ImageUrl = newFileName;
            }
            data.Link = vm.Link;
            data.Subtitle = vm.Subtitle;
            data.Title = vm.Title;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();


            string oldFilePath = Path.Combine(_env.WebRootPath, "imgs", "sliders", data.ImageUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            _context.Sliders.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int id)
        {
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Show(int id)
        {
            var data = await _context.Sliders.FindAsync(id);

            if (data is null) return View();

            data.IsDeleted = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }







    }
}
