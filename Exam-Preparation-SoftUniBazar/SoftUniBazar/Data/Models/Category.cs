using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.Common.Constraints.CategoryConstraints;
namespace SoftUniBazar.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(categoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Ad> Ads { get; set; } = new HashSet<Ad>();
    }
}
