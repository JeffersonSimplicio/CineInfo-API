using ISession = CineInfo_API.Interfaces.ISession;

namespace CineInfo_API.Data.DTOs; 
public class InputSessionDTO : ISession {
    public int MovieId { get; set; }
    public int CinemaId { get; set; }
    public DateTime StartTime { get; set; }
    public double TicketPrice { get; set; }
}
