using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.Common.DataConstants;

namespace SeminarHub.Models
{
    public class SeminarAddViewModel
    {
        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(TopicNameMaximumLength,
            MinimumLength = TopicNameMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(LecturerMaximumLength, 
            MinimumLength = LecturerMinimumLength, 
            ErrorMessage = StringLengthErrorMessage)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        [StringLength(DetailsMaximumLength,
            MinimumLength = DetailsMinimumLength,
            ErrorMessage = StringLengthErrorMessage)]
        public string Details { get; set; } = string.Empty;

        [Required(ErrorMessage = RequireErrorMessage)]
        public string DateAndTime { get; set; } = string.Empty;

        [Range(DurationMinimum, DurationMaximum, 
            ErrorMessage = StringLengthErrorMessage)]
        public int Duration { get; set; }

        [Required(ErrorMessage = RequireErrorMessage)]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
