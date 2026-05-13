using AutoMapper;
using kitap.Core.Results;
using kitap.Core.UnitOfWork;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IReservationService
    {
        Task<IDataResult<IEnumerable<ReservationDto>>> GetActiveReservationsAsync();
        Task<IResult> CreateReservationAsync(int bookId, int userId);
        Task<IResult> CancelReservationAsync(int reservationId);
    }

    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ReservationService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IDataResult<IEnumerable<ReservationDto>>> GetActiveReservationsAsync()
        {
            var reservations = await _uow.Repository<Reservation>().GetQueryable()
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Status == ReservationStatus.Pending)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);
            return new SuccessDataResult<IEnumerable<ReservationDto>>(dtos);
        }

        public async Task<IResult> CreateReservationAsync(int bookId, int userId)
        {
            // Kitap zaten ödünçte mi kontrol et
            var isBorrowed = await _uow.Repository<Borrowing>().GetQueryable()
                .AnyAsync(b => b.BookId == bookId && !b.IsReturned);

            if (!isBorrowed)
            {
                return new ErrorResult("Kitap şu an müsait, rezervasyon yerine doğrudan ödünç alabilirsiniz.");
            }

            var reservation = new Reservation
            {
                BookId = bookId,
                UserId = userId,
                ReservationDate = DateTime.UtcNow,
                Status = ReservationStatus.Pending
            };

            await _uow.Repository<Reservation>().AddAsync(reservation);
            await _uow.SaveChangesAsync();

            return new SuccessResult("Rezervasyon sırasına alındınız. Kitap iade edildiğinde size bildirim yapılacak.");
        }

        public async Task<IResult> CancelReservationAsync(int reservationId)
        {
            var res = await _uow.Repository<Reservation>().GetByIdAsync(reservationId);
            if (res == null) return new ErrorResult("Rezervasyon bulunamadı.");

            res.Status = ReservationStatus.Cancelled;
            _uow.Repository<Reservation>().Update(res);
            await _uow.SaveChangesAsync();

            return new SuccessResult("Rezervasyon iptal edildi.");
        }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
