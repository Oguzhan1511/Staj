using System.ComponentModel.DataAnnotations;

namespace kitap.Dtos
{
    public class PublisherCreateDto
    {
        [Required(ErrorMessage = "Yayıncı ismi boş bırakılamaz.")]
        public string Name { get; set; } = string.Empty;
    }
}
