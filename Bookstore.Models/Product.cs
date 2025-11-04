using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Titel")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ISBN")]
        [RegularExpression(@"^(\d{10}|\d{13})$", ErrorMessage = "ISBN moet 10 of 13 cijfers bevatten")]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Auteur")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Beschrijving")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Prijs")]
        [Range(1, 10000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Prijs 1-50")]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Prijs 51-100")]
        [Range(1, 10000)]
        public double Price50 { get; set; }

        [Required]
        [Display(Name = "Prijs 100+")]
        [Range(1, 10000)]
        public double Price100 { get; set; }

        [Display(Name = "Afbeelding")]
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation properties
        [Required]
        [Display(Name = "Categorie")]
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [Display(Name = "Soort kaft")]
        public int CoverTypeId { get; set; }
        
        [ForeignKey("CoverTypeId")]
        public CoverType? CoverType { get; set; }
    }
}
