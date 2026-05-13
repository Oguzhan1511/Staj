using System;

namespace kitap.Dtos
{
    public class BorrowingDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }

    public class BorrowCreateDto
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int DaysToBorrow { get; set; } = 14; // Varsayılan 14 gün
    }
}
