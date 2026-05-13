using System.ComponentModel.DataAnnotations;

namespace kitap.Dtos
{
    public class BooksCreateDto
    {
        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yazar alanı boş bırakılamaz.")]
        public int AuthorId { get; set; } 

        [Required(ErrorMessage = "Yayınevi alanı boş bırakılamaz")]
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Kategori alanı boş bırakılamaz")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Yıl alanı boş bırakılamaz")]
        public int PublisherYear { get; set; } 
    }
}