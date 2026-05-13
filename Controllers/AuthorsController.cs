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
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var authors = await _authorService.GetAllAsync(pageNumber, pageSize);
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> PostAuthor(AuthorCreateDto authorDto)
        {
            var createdAuthor = await _authorService.CreateAsync(authorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorCreateDto authorDto)
        {
            var result = await _authorService.UpdateAsync(id, authorDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
