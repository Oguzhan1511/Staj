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
        public async Task<IActionResult> GetActiveBorrowings()
        {
            var result = await _borrowingService.GetActiveBorrowingsAsync();
            return Ok(result);
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(BorrowCreateDto borrowDto)
        {
            var result = await _borrowingService.BorrowBookAsync(borrowDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var result = await _borrowingService.ReturnBookAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{userId}/history")]
        public async Task<IActionResult> GetUserHistory(int userId)
        {
            var result = await _borrowingService.GetUserHistoryAsync(userId);
            return Ok(result);
        }
    }
}
