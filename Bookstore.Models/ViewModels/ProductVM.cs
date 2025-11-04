using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = new Product();
        public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CoverTypeList { get; set; } = new List<SelectListItem>();
    }
}
