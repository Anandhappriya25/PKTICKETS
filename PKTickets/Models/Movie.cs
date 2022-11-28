using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PKTickets.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please Enter the Title !!!")]
        [StringLength(20, ErrorMessage = "Title length must be below 20 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please Enter the Duration in Minutes !!!")]
        [Range(60, 300, ErrorMessage = "Duration must be between 60 to 300 Minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Please Enter the CastAndCrew !!!")]
        [StringLength(50, ErrorMessage = "CastAndCrew length must be below 50 characters")]
        public string CastAndCrew { get; set; }

        [Required(ErrorMessage = "Please Enter the Language !!!")]
        [StringLength(20, ErrorMessage = "Language length must be below 20 characters")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Please Enter the Director Name !!!")]
        [StringLength(20, ErrorMessage = "Director Name length must be below 20 characters")]
        public string Director { get; set; }
        [Required(ErrorMessage = "Please Enter the Genre !!!")]
        [StringLength(20, ErrorMessage = "Genre length must be below 20 characters")]
        public string Genre { get; set; }
        public bool IsPlaying { get; set; }=true;
        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile? CoverPhoto { get; set; }
    }
}
