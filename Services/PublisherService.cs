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
    public class PublisherService : IPublisherService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PublisherService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PublisherDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageSize > 50) pageSize = 50;

            var publishers = await _context.Publishers
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto?> GetByIdAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return null;

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<PublisherDto> CreateAsync(PublisherCreateDto publisherDto)
        {
            var publisher = _mapper.Map<Publisher>(publisherDto);
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task<bool> UpdateAsync(int id, PublisherCreateDto publisherDto)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return false;

            _mapper.Map(publisherDto, publisher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return false;

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
