using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        
        [Range(1, 1000, ErrorMessage = "Aantal moet tussen 1 en 1000 zijn")]
        public int Count { get; set; }
        
        public string ApplicationUserId { get; set; } = string.Empty;
        
        [NotMapped]
        public double Price { get; set; }
    }
}
