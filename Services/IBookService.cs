using kitap.Core.Results;
using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IBookService
    {
        Task<IDataResult<IEnumerable<BooksDto>>> GetAllAsync(
            string? searchTerm, 
            int? categoryId, 
            int? authorId, 
            string? sortBy, 
            int pageNumber, 
            int pageSize);
            
        Task<IDataResult<BooksDto>> GetByIdAsync(int id);
        Task<IDataResult<BooksDto>> CreateAsync(BooksCreateDto bookDto);
        Task<IResult> UpdateAsync(int id, BooksCreateDto bookDto);
        Task<IResult> DeleteAsync(int id);
        Task<IResult> UpdateImageAsync(int id, string imageUrl);
    }
}
