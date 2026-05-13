using System.Collections.Generic;

namespace kitap.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for books in this category
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
