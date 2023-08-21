using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs;
public class InputMovieDTO : IMovie {
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
}
