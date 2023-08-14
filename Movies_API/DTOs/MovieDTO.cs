using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTOs {
    public class MovieDTO {
        [Required]
        public string Title { get; set; }
        [Required]
        public int ReleaseYear { get; set; }
        [Required]
        public int Duration { get; set; }
    }
}
