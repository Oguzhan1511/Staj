using Microsoft.EntityFrameworkCore;
using kitap.Models;


namespace kitap.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }
    public DbSet<Review> Reviews { get; set; }


/*    internal async Task UpdateAsync()
{
    throw new NotImplementedException();
}
*/
   
}
