namespace CineInfo_API.Interfaces; 
public interface ISession {
    public int MovieId { get; set; }
    public int CinemaId { get; set; }
    public DateTime StartTime { get; set; }
    public double TicketPrice { get; set; }
}
