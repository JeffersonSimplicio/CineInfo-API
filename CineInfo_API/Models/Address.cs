using CineInfo_API.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineInfo_API.Models;
public class Address : IAddress {
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("neighborhood")]
    public string Neighborhood { get; set; }
    [Required]
    [Column("street")]
    public string Street { get; set; }
    [Required]
    [Column("number")]
    public int Number { get; set; }
    public virtual Cinema Cinema { get; set; }
}
