using System.ComponentModel.DataAnnotations;

namespace Movies_API.Models;

public class Movie {
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    [Required]
    public int Duration { get; set; }
    //[Required]
    //public HashSet<string> Genre { get; set; }

    //public override string ToString() {
    //    string genresList = string.Join(", ", Genre);
    //    return $"Titulo: {Title}\nAno: {ReleaseYear}\nDuração:{Duration} minutos\nGenero: {genresList}";
    //}
}