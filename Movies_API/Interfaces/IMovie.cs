namespace Movies_API.Interfaces; 
public interface IMovie {
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public int Duration { get; set; }
}
