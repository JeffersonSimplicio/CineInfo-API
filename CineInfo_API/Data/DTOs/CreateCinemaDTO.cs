using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs;
public class CreateCinemaDTO : ICinema {
    public string Name { get; set; }
    public int NumberHalls { get; set; }
    public int AddressID { get; set; }
}
