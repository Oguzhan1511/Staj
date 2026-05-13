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
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageSize > 50) pageSize = 50;

            var authors = await _context.Authors
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }

        public async Task<AuthorDto?> GetByIdAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return null;

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<AuthorDto> CreateAsync(AuthorCreateDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<bool> UpdateAsync(int id, AuthorCreateDto authorDto)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            _mapper.Map(authorDto, author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
