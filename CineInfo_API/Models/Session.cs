using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ISession = CineInfo_API.Interfaces.ISession;

namespace CineInfo_API.Models;
public class Session : ISession {
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("start_time")]
    public DateTime StartTime { get; set; }
    [Required]
    [Column("ticket_price")]
    public double TicketPrice { get; set; }
    [Required]
    [Column("movie_id")]
    public int MovieId { get; set; }
    public virtual Movie Movie { get; set; }
    [Required]
    [Column("cinema_id")]
    public int CinemaId { get; set; }
    public virtual Cinema Cinema{ get; set; }
}
