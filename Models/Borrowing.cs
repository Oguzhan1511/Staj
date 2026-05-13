using System;

namespace kitap.Models
{
    public class Borrowing
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime ReturnDate { get; set; } 
        public DateTime? ActualReturnDate { get; set; } 
        public bool IsReturned { get; set; } = false;

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
