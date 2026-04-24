using System.Collections.Generic;

namespace libraryApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Bir kategorinin birden fazla kitabı olabilir
        public List<Book> Books { get; set; } = new List<Book>();
    }
}