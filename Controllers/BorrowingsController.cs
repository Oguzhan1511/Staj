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
    public class BorrowingsController : ControllerBase
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingsController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BorrowingDto>>> GetActiveBorrowings()
        {
            var borrowings = await _borrowingService.GetActiveBorrowingsAsync();
            return Ok(borrowings);
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<BorrowingDto>> BorrowBook(BorrowCreateDto borrowDto)
        {
            try
            {
                var result = await _borrowingService.BorrowBookAsync(borrowDto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var result = await _borrowingService.ReturnBookAsync(id);
            if (!result) return NotFound("Ödünç kaydı bulunamadı veya kitap zaten iade edilmiş.");
            return Ok("Kitap başarıyla iade edildi.");
        }

        [HttpGet("user/{userId}/history")]
        public async Task<ActionResult<IEnumerable<BorrowingDto>>> GetUserHistory(int userId)
        {
            var history = await _borrowingService.GetUserHistoryAsync(userId);
            return Ok(history);
        }
    }
}
