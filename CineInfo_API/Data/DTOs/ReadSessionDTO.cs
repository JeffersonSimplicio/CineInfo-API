using ISession = CineInfo_API.Interfaces.ISession;

namespace CineInfo_API.Data.DTOs; 
public class ReadSessionDTO : ISession {
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int CinemaId { get; set; }
    public DateTime StartTime { get; set; }
    public double TicketPrice { get; set; }
    public string SearchTimestamp { get; set; } = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
}
