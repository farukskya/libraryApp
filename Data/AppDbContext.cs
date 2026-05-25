using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using libraryApp.Models;

namespace libraryApp.Data
{
    // DbContext yerine artık IdentityDbContext kullanıyoruz, böylece Identity tabloları da eklenecek
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Geçen haftalardan kalan kitap ve kategori tablolarımız aynen duruyor
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity kurallarının çalışması için bu satır ŞART!

            // Eski Book - Category ilişki yapısını aynen koruyoruz
            builder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId);
        }
    }
}