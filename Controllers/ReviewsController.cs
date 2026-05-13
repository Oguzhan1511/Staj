using kitap.Dtos;
using kitap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetBookReviews(int bookId)
        {
            var reviews = await _reviewService.GetByBookIdAsync(bookId);
            return Ok(reviews);
        }

        [HttpGet("book/{bookId}/average")]
        public async Task<ActionResult<double>> GetAverageRating(int bookId)
        {
            var average = await _reviewService.GetAverageRatingAsync(bookId);
            return Ok(average);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> PostReview(ReviewCreateDto reviewDto)
        {
            var result = await _reviewService.AddReviewAsync(reviewDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
