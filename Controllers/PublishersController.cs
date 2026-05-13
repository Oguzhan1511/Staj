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
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers(int pageNumber = 1, int pageSize = 10)
        {
            var publishers = await _publisherService.GetAllAsync(pageNumber, pageSize);
            return Ok(publishers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisher(int id)
        {
            var publisher = await _publisherService.GetByIdAsync(id);
            if (publisher == null) return NotFound();
            return Ok(publisher);
        }

        [HttpPost]
        public async Task<ActionResult<PublisherDto>> PostPublisher(PublisherCreateDto publisherDto)
        {
            var createdPublisher = await _publisherService.CreateAsync(publisherDto);
            return CreatedAtAction(nameof(GetPublisher), new { id = createdPublisher.Id }, createdPublisher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, PublisherCreateDto publisherDto)
        {
            var result = await _publisherService.UpdateAsync(id, publisherDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var result = await _publisherService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
