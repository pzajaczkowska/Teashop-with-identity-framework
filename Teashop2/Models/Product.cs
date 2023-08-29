using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teashop2.Models
{
    public class Product
    {
        public Product() =>
            Categories = new HashSet<Category>();

        public int Id { get; set; }

        [StringLength(20)]
        [Required]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        [Range(1, 500)]
        [Required]
        public float Price { get; set; }

        [Range(0, 5000)]
        [Required]
        public int QuantityOnStock { get; set; }

        public bool IsAvaliable { get; set; }

        public ICollection<Category> Categories { get; set; }

        public string? ImagePath { get; set; }
    }
}
