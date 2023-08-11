namespace Movies_API.Models;

public class Movie {
    public int Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
    public HashSet<string> Genre { get; set; }

    public override string ToString() {
        string genresList = string.Join(", ", Genre);
        return $"Titulo: {Title}\nAno: {ReleaseYear}\nDuração:{Duration} minutos\nGenero: {genresList}";
    }
}