using kitap.Models;

namespace kitap.Models
{
    public class Book {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public int AuthorId { get; set; } 
    public int PublisherId { get; set; }
    public int CategoryId { get; set; }
    public int PublisherYear { get; set; }
    public string? ImageUrl { get; set; }

    public Author Author { get; set; } = null!;
    public Publisher Publisher { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
}

