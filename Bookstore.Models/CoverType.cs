using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class CoverType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Soort kaft")]
        public string Name { get; set; } = string.Empty;
    }
}
