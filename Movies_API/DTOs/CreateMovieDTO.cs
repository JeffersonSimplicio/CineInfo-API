using Movies_API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Movies_API.DTOs; 
public class CreateMovieDTO: IMovie {
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
}
