namespace CineInfo_API.Interfaces; 
public interface IAddress {
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
}
