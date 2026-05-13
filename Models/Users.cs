using System.ComponentModel.DataAnnotations;
namespace kitap.Models;

public class User
{
    public int Id { get; set;}
    public string Name { get; set;} = string.Empty;
    public string LastName { get; set;} = string.Empty;  
    public string Email {get; set;} = string.Empty;
    public string Password { get; set;} = string.Empty; 

}
