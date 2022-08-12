using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

public class Movie
{
    [Key]
    public int MovieId { get; set; }
    [Required]
    public string Tittle {get;set;}
    [Required]
    public string Description { get; set; }
    [Required]
    public int UserId { get; set; }
    // Navigation property for related User object
    public User? Creator { get; set; }
    // public List<Like> Likers { get; set; } = new List<Like>(); 
    public List<Fans> Fansat { get; set; } = new List<Fans>(); 

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


}