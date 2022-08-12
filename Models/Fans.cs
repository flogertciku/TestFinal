using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;


public class Fans{
    [Key]
    public int FansId {get;set;}
    public int UserId {get;set;}
    public int MovieId { get; set; }
    public string Type{get;set;}
    public User? UseriQePelqen {get;set;}
    public Movie? FilmiQePelqehet {get;set;}
    
}