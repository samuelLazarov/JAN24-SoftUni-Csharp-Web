namespace TaskBoard.Models;
using System.ComponentModel.DataAnnotations;
using Task = TaskBoard.Data.Task;

    public class BoardViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public IEnumerable<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }

