using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniqloMVC1.DataAccess;
using UniqloMVC1.ViewModels.Baskets;

namespace UniqloMVC1.ViewComponents
{
    public class HeaderViewComponent(UniqloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketIds = JsonSerializer.Deserialize<List<BasketItemVM>>(Request.Cookies["basket"] ?? "[]");

            var products = await _context.Products
                .Where(x => basketIds!
                .Select(x => x.Id).Any(y => y == x.Id))
                .Select(x => new BasketItemVM
                {
                    Id = x.Id,
                    Discount = x.Discount,
                    ImageUrl = x.CoverImage,
                    SellPrice = x.SellPrice,
                    Name = x.Name
                }).ToListAsync();


            foreach (var item in products)
            {
                item.Count = basketIds!.FirstOrDefault(x => x.Id == item.Id)!.Count;
            }
            return View(products);
        }
    }
}
