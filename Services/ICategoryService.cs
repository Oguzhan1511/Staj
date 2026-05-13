using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryCreateDto categoryDto);
        Task<bool> UpdateAsync(int id, CategoryCreateDto categoryDto);
        Task<bool> DeleteAsync(int id);
    }
}
