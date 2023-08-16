﻿using Movies_API.Interfaces;

namespace Movies_API.DTOs; 
public class ReadMovieDTO: IMovie {
    public int Id { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
    public string SearchTimestamp { get; set; } = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
}
