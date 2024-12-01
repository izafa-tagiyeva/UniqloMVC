using System.ComponentModel.DataAnnotations;
using UniqloMVC1.Models;

namespace UniqloMVC1.ViewModels.Category
{
    public class CategoryCreateVM:BaseEntity
    {
        [MaxLength(32, ErrorMessage = "Name must be less than 32"), Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
    }
}
