using CineInfo_API.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CineInfo_API.Models;

public class Movie : IMovie {
    [Key]
    [Required]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("title")]
    public string Title { get; set; }
    [Required]
    [Column("release_year")]
    public int ReleaseYear { get; set; }
    [Required]
    [Column("duration")]
    public int Duration { get; set; }
}