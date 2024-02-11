using System.ComponentModel.DataAnnotations;
using static TaskBoard.Data.DataConstants.Task;

namespace TaskBoard.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLenght)]
        [MinLength(TitleMinLenght)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(DescriptionMaxLenght)]
        [MinLength(DescriptionMinLenght)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Owner { get; set; } = string.Empty;
    }
}
