using System.Drawing.Drawing2D;
using UniqloMVC1.Models;
using UniqloMVC1.ViewModels.Slider;
using UniqloMVC1.ViewModels.Product;

namespace UniqloMVC1.ViewModels.Commons
{
    public class HomeVM
    {
        public IEnumerable<SliderItemVM> Sliders { get; set; }
        public IEnumerable<ProductItemVM> Products { get; set; }
    }
}
