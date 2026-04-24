using libraryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace libraryApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}