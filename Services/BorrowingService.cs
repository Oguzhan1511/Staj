using AutoMapper;
using kitap.Data;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BorrowingService> _logger;

        public BorrowingService(AppDbContext context, IMapper mapper, ILogger<BorrowingService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BorrowingDto>> GetActiveBorrowingsAsync()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.IsReturned)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BorrowingDto>>(borrowings);
        }

        public async Task<BorrowingDto> BorrowBookAsync(BorrowCreateDto borrowDto)
        {
            // Kitap şu an başkasında mı kontrol et
            var isAlreadyBorrowed = await _context.Borrowings
                .AnyAsync(b => b.BookId == borrowDto.BookId && !b.IsReturned);

            if (isAlreadyBorrowed)
            {
                throw new Exception("Bu kitap şu an ödünç verilmiş durumda.");
            }

            var borrowing = _mapper.Map<Borrowing>(borrowDto);
            borrowing.BorrowDate = DateTime.UtcNow;
            borrowing.IsReturned = false;

            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Kitap ödünç verildi. KitapId: {BookId}, KullanıcıId: {UserId}", borrowDto.BookId, borrowDto.UserId);

            // Navigasyon property'lerini doldurmak için tekrar çekiyoruz
            var result = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstAsync(b => b.Id == borrowing.Id);

            return _mapper.Map<BorrowingDto>(result);
        }

        public async Task<bool> ReturnBookAsync(int borrowingId)
        {
            var borrowing = await _context.Borrowings.FindAsync(borrowingId);
            if (borrowing == null || borrowing.IsReturned) return false;

            borrowing.IsReturned = true;
            borrowing.ActualReturnDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Kitap iade edildi. ÖdünçId: {BorrowingId}", borrowingId);
            return true;
        }

        public async Task<IEnumerable<BorrowingDto>> GetUserHistoryAsync(int userId)
        {
            var history = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<BorrowingDto>>(history);
        }
    }
}
