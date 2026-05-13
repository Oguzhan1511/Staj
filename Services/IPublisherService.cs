using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PublisherDto?> GetByIdAsync(int id);
        Task<PublisherDto> CreateAsync(PublisherCreateDto publisherDto);
        Task<bool> UpdateAsync(int id, PublisherCreateDto publisherDto);
        Task<bool> DeleteAsync(int id);
    }
}
