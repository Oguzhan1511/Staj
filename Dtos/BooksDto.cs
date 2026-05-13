


namespace kitap.Dtos
{
    public class BooksDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        
        public int AuthorId { get; set; } 
        public int PublisherId { get; set; }
        public int CategoryId { get; set; }
        public int PublisherYear { get; set; }
        public string? ImageUrl { get; set; }
    }
}
