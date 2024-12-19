using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqloMVC1.DataAccess;
using UniqloMVC1.ViewModels.Baskets;

namespace UniqloMVC1.Controllers
{
    public class ProductController(UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return BadRequest();

            var data = await _context.Products
                .Where(x => x.Id == id)
                .Include(x => x.Ratings)
                .Include(x => x.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();

            if (data is null) return NotFound();


            ViewBag.Rating = 5;
            if (User.Identity?.IsAuthenticated ?? false)
            {
                string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

                int rating = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.Rating).FirstOrDefaultAsync();
                ViewBag.Rating = rating == 0 ? 5 : rating;
            }

            return View(data);
        }
        public async Task<IActionResult> Rating(int productId, int rating)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.ProductRatings.Where(x => x.UserId == userId && x.ProductId == productId).FirstOrDefaultAsync();

            if (data is null)
            {
                await _context.ProductRatings.AddAsync(new Models.ProductRating
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating
                });
            }
            else
            {
                data.Rating = rating;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = productId });
        }


        /// ////////////////////////////////////////////////////////////////////////////////////////

        public async Task<IActionResult> Comment(int productId, string comment, string name)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
            var data = await _context.ProductComments
                .Where(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefaultAsync();
            await _context.ProductComments.AddAsync(new Models.ProductComment
            {
                UserId = userId,
                Comment = comment,
                ProductId = productId,
                Name = name
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { Id = productId });
        }
        public async Task<IActionResult> RemoveComment(int? id)
        {
            if (!id.HasValue) return BadRequest();
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

            var data = await _context.ProductComments.FindAsync(id);

            ViewBag.UserId = userId;

            if (data is null) return NotFound();

            _context.ProductComments.Remove(data);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { Id = data.ProductId });
        }
        ////////////////////////////////////////////////////////////////////
       
          public async Task<IActionResult> AddBasket(int id)
           {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();

            var basketItems = JsonSerializer.Deserialize<List<BasketCookieItemVM>>(Request.Cookies["basket"] ?? "[]");

            var item = basketItems.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                item = new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                };
                basketItems.Add(item);
            }
            item.Count++;

            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));

            return Ok();
           }

        public async Task<IActionResult> RemoveBasket(int id)
        {
          
            if (!await _context.Products.AnyAsync(x => x.Id == id))
                return NotFound();

            var basketCookie = Request.Cookies["basket"];
            List<BasketCookieItemVM> basketItems;

            try
            {
                basketItems = JsonSerializer.Deserialize<List<BasketCookieItemVM>>(basketCookie ?? "[]");
            }
            catch
            {
                return BadRequest("Invalid basket data.");
            }

            var item = basketItems.FirstOrDefault(x => x.Id == id);

            if (item is null)
                return NotFound("Item not found in basket.");

            
            item.Count--;
            if (item.Count <= 0)
            {
                basketItems.Remove(item);
            }

           
            Response.Cookies.Append("basket", JsonSerializer.Serialize(basketItems));

            return RedirectToAction("Index","Home");
        }


    }
}
