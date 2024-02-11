using System.ComponentModel.DataAnnotations;
using static TaskBoard.Data.DataConstants.Task;
using static TaskBoard.Models.ErrorMessages;

namespace TaskBoard.Models
{
    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = RequireError)]
        [StringLength(TitleMaxLenght, MinimumLength = TitleMinLenght, 
            ErrorMessage = StringLengthError)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireError)]
        [StringLength(DescriptionMaxLenght, MinimumLength = DescriptionMinLenght,
            ErrorMessage = StringLengthError)]
        public string Description { get; set; } = string.Empty;

        public int? BoardId { get; set; }

        public IEnumerable<TaskBoardModel> Boards { get; set; } = new List<TaskBoardModel>();
    }
}
