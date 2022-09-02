using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

public class Request {

[Key]
public int RequestId {get;set;}
public bool Accepted {get;set;} = false;
public int SenderId {get;set;}
public User? Sender {get;set;}
public int ReciverId{get;set;}
[NotMapped]
public User? Reciver {get;set;}
public DateTime CreatedAt { get; set; } = DateTime.Now;
public DateTime UpdatedAt { get; set; } = DateTime.Now;

}