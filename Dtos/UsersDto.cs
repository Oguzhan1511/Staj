using System.ComponentModel.DataAnnotations;

namespace kitap.Dtos
{
    public class UsersDto
    {
        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;
    }
}