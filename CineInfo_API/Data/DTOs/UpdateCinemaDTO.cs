using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs;
public class UpdateCinemaDTO : ICinema {
    public string Name { get; set; }
    public int NumberHalls { get; set; }
}
