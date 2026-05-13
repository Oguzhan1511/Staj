using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetByBookIdAsync(int bookId);
        Task<ReviewDto> AddReviewAsync(ReviewCreateDto reviewDto);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<bool> DeleteReviewAsync(int reviewId);
    }
}
