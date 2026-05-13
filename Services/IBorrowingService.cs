using kitap.Core.Results;
using kitap.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace kitap.Services
{
    public interface IBorrowingService
    {
        Task<IDataResult<IEnumerable<BorrowingDto>>> GetActiveBorrowingsAsync();
        Task<IDataResult<BorrowingDto>> BorrowBookAsync(BorrowCreateDto borrowDto);
        Task<IResult> ReturnBookAsync(int borrowingId);
        Task<IDataResult<IEnumerable<BorrowingDto>>> GetUserHistoryAsync(int userId);
    }
}
