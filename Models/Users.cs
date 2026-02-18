using System.ComponentModel.DataAnnotations;
namespace kitap.Models;

public class User
{
    public int Id { get; set;}
    
    [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
    public string Name { get; set;} = string.Empty;
    [Required(ErrorMessage ="Soyad alanı boş bırakılamaz.")]
    public string LastName { get; set;} = string.Empty;
    [Required(ErrorMessage ="E-posta adresi boş bırakılamaz")]
    [EmailAddress(ErrorMessage ="Geçersiz e-posta formatı")]
    public string Mail {get; set;} = string.Empty;
    [Required(ErrorMessage ="Şifre alanı boş bırakılamaz")]
    [MinLength(6, ErrorMessage ="Şifre en az 6 karakterli olmalıdır")]
    public string Password { get; set;} = string.Empty; 
    
}
