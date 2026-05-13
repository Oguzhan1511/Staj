using System;

namespace kitap.Models
{
    public enum ReservationStatus { Pending, Completed, Cancelled }

    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
