using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IBorrowingService
    {
        Task<IEnumerable<BorrowingDto>> GetActiveBorrowingsAsync();
        Task<BorrowingDto> BorrowBookAsync(BorrowCreateDto borrowDto);
        Task<bool> ReturnBookAsync(int borrowingId);
        Task<IEnumerable<BorrowingDto>> GetUserHistoryAsync(int userId);
    }
}
