using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeminarHub.Data.Common.DataConstants;

namespace SeminarHub.Data.Models
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TopicNameMaximumLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [MaxLength(LecturerMaximumLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MaxLength(DetailsMaximumLength)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        public DateTime DateAndTime { get; set; }

        [Range(DurationMinimum, DurationMaximum)]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}
