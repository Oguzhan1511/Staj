using System.ComponentModel.DataAnnotations;

namespace kitap.Dtos
{
    public class AuthorCreateDto
    {
        [Required(ErrorMessage = "İsim alanı boş bırakılamaz.")]
        public string Name { get; set; } = string.Empty;
    }
}
