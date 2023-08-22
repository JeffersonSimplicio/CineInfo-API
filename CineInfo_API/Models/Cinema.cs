using CineInfo_API.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineInfo_API.Models;
public class Cinema : ICinema {
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    public string Name { get; set; }
    [Required]
    [Column("number_halls")]
    public int NumberHalls { get; set; }
    [Required]
    [Column("address_id")]
    public int AddressId { get; set; }
    public virtual Address Address { get; set; }
}
