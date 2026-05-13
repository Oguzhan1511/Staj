using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using kitap.Dtos;
using kitap.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IFileService _fileService;

        public BooksController(IBookService bookService, IFileService fileService)
        {
            _bookService = bookService;
            _fileService = fileService;
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var bookResult = await _bookService.GetByIdAsync(id);
            if (!bookResult.Success) return NotFound(bookResult.Message);

            var imagePath = await _fileService.SaveFileAsync(file, "images");
            
            if (!string.IsNullOrEmpty(bookResult.Data.ImageUrl))
                _fileService.DeleteFile(bookResult.Data.ImageUrl);

            var result = await _bookService.UpdateImageAsync(id, imagePath);
            if (!result.Success) return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] string? search, 
            [FromQuery] int? categoryId, 
            [FromQuery] int? authorId, 
            [FromQuery] string? sortBy,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            var result = await _bookService.GetAllAsync(search, categoryId, authorId, sortBy, pageNumber, pageSize);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var result = await _bookService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostBook(BooksCreateDto bookDto)
        {
            var result = await _bookService.CreateAsync(bookDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BooksCreateDto bookDto)
        {
            var result = await _bookService.UpdateAsync(id, bookDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}