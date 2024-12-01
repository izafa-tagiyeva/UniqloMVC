using System.ComponentModel.DataAnnotations;

namespace UniqloMVC1.ViewModels.Product
{
    public class ProductUpdateVM
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Cost Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost Price must be greater than 0.")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "Sell Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sell Price must be greater than 0.")]
        public decimal SellPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public int Discount { get; set; }


        public IFormFile? CoverFile { get; set; }

        public int? CategoryId { get; set; }

    }
}
