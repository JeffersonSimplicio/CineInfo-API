using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs;
public class ReadCinemaDTO : ICinema {
    public int Id { get; set; }
    public string Name { get; set; }
    public int NumberHalls { get; set; }
    public string SearchTimestamp { get; set; } = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
}
