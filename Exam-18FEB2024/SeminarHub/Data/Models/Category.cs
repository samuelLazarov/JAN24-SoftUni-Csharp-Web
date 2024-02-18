using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.Common.DataConstants;

namespace SeminarHub.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaximumLength)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Seminar> Seminars { get; set; } = new List<Seminar>();
    }
}
