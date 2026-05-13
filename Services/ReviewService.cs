using AutoMapper;
using kitap.Data;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReviewService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetByBookIdAsync(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var result = await _context.Reviews
                .Include(r => r.User)
                .FirstAsync(r => r.Id == review.Id);

            return _mapper.Map<ReviewDto>(result);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var reviews = await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
