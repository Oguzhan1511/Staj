using kitap.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Jobs
{
    public class OverdueBookJob
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OverdueBookJob> _logger;

        public OverdueBookJob(AppDbContext context, ILogger<OverdueBookJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckOverdueBooksAsync()
        {
            _logger.LogInformation("Geciken kitaplar kontrol ediliyor...");

            var overdueBorrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.IsReturned && b.ReturnDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var item in overdueBorrowings)
            {
                // Burada mail gönderim kodu simüle edilebilir
                _logger.LogWarning("GECİKME BİLDİRİMİ: Kullanıcı {UserName}, {BookTitle} kitabını {ReturnDate} tarihinde iade etmeliydi!", 
                    item.User.Name, item.Book.Title, item.ReturnDate);
            }

            if (!overdueBorrowings.Any())
            {
                _logger.LogInformation("Geciken kitap bulunamadı.");
            }
        }
    }
}
