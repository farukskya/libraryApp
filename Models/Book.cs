using System.ComponentModel.DataAnnotations;

namespace libraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı boş bırakılamaz.")]
        [StringLength(100, ErrorMessage = "Kitap adı en fazla 100 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Yazar adı boş bırakılamaz.")]
        [StringLength(50, ErrorMessage = "Yazar adı en fazla 50 karakter olabilir.")]
        public string Author { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı 0 veya daha büyük olmalıdır.")]
        public int Stock { get; set; }
    }
}