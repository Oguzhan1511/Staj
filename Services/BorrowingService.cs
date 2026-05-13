using AutoMapper;
using kitap.Core.Results;
using kitap.Core.UnitOfWork;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<BorrowingService> _logger;

        public BorrowingService(IUnitOfWork uow, IMapper mapper, ILogger<BorrowingService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IDataResult<IEnumerable<BorrowingDto>>> GetActiveBorrowingsAsync()
        {
            var borrowings = await _uow.Repository<Borrowing>().GetQueryable()
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.IsReturned)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<BorrowingDto>>(borrowings);
            return new SuccessDataResult<IEnumerable<BorrowingDto>>(dtos);
        }

        public async Task<IDataResult<BorrowingDto>> BorrowBookAsync(BorrowCreateDto borrowDto)
        {
            var isAlreadyBorrowed = await _uow.Repository<Borrowing>().GetQueryable()
                .AnyAsync(b => b.BookId == borrowDto.BookId && !b.IsReturned);

            if (isAlreadyBorrowed)
            {
                return new ErrorDataResult<BorrowingDto>("Bu kitap şu an ödünç verilmiş durumda.");
            }

            var borrowing = _mapper.Map<Borrowing>(borrowDto);
            borrowing.BorrowDate = DateTime.UtcNow;
            borrowing.IsReturned = false;

            await _uow.Repository<Borrowing>().AddAsync(borrowing);
            await _uow.SaveChangesAsync();

            var result = await _uow.Repository<Borrowing>().GetQueryable()
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstAsync(b => b.Id == borrowing.Id);

            var dto = _mapper.Map<BorrowingDto>(result);
            return new SuccessDataResult<BorrowingDto>(dto, "Kitap başarıyla ödünç verildi.");
        }

        public async Task<IResult> ReturnBookAsync(int borrowingId)
        {
            var borrowing = await _uow.Repository<Borrowing>().GetByIdAsync(borrowingId);
            if (borrowing == null || borrowing.IsReturned) return new ErrorResult("Geçersiz ödünç kaydı.");

            borrowing.IsReturned = true;
            borrowing.ActualReturnDate = DateTime.UtcNow;

            // Ceza Hesaplama Mantığı
            if (borrowing.ActualReturnDate > borrowing.ReturnDate)
            {
                var lateDays = (borrowing.ActualReturnDate.Value - borrowing.ReturnDate).Days;
                if (lateDays > 0)
                {
                    borrowing.FineAmount = lateDays * 5; // Günlük 5 TL
                }
            }

            _uow.Repository<Borrowing>().Update(borrowing);
            await _uow.SaveChangesAsync();

            string message = borrowing.FineAmount > 0 
                ? $"Kitap iade edildi. Gecikme cezası: {borrowing.FineAmount} TL" 
                : "Kitap başarıyla iade edildi.";

            return new SuccessResult(message);
        }

        public async Task<IDataResult<IEnumerable<BorrowingDto>>> GetUserHistoryAsync(int userId)
        {
            var history = await _uow.Repository<Borrowing>().GetQueryable()
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<BorrowingDto>>(history);
            return new SuccessDataResult<IEnumerable<BorrowingDto>>(dtos);
        }
    }
}
