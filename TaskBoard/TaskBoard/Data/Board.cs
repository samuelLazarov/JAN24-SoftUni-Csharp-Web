using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TaskBoard.Data.DataConstants.Board;

namespace TaskBoard.Data
{
    public class Board
    {

        [Required]
        [Comment("Board identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Board name")]
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Task> Tasks = new List<Task>();
    }
}