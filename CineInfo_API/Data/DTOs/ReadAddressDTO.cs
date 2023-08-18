using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs;
public class ReadAddressDTO : IAddress {
    public int Id { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
}
