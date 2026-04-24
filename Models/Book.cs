using System.ComponentModel.DataAnnotations;

namespace libraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        public string Author { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok 0 veya daha büyük olmalıdır.")]
        public int Stock { get; set; }

        // 9. HAFTA EKLEMELERİ:
        public int CategoryId { get; set; } // Foreign Key [cite: 180]
        public Category Category { get; set; } // Navigation Property [cite: 181]
    }
}