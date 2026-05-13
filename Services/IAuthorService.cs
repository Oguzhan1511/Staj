using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<AuthorDto?> GetByIdAsync(int id);
        Task<AuthorDto> CreateAsync(AuthorCreateDto authorDto);
        Task<bool> UpdateAsync(int id, AuthorCreateDto authorDto);
        Task<bool> DeleteAsync(int id);
    }
}
