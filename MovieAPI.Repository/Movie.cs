using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Repository
{
    public class Movie
    {
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Classification { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        [RegularExpression(@"[12]\d{3}")]//[Range(1800, 2099)]//
        public string ReleaseDate { get; set; }
        [Required]
        [Range(1,5)]
        public int Rating { get; set; }
        public string[] Cast { get; set; }
    }
}