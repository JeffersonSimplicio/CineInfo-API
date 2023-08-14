using Movies_API.Interfaces;

namespace Movies_API.DTOs; 
public class UpdateMovieDTO: IMovie {
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
}
