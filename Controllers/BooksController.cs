using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using libraryApp.Data;
using libraryApp.Models;

namespace libraryApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // KİTAP LİSTESİ
        public IActionResult Index(string searchString, int? categoryId, string sortOrder)
        {
            // 1. Kategorileri dropdown (filtre) için gönderiyoruz
            ViewBag.Categories = _context.Categories.ToList();

            // 2. Mevcut seçimleri sayfada tutmak için ViewBag'e atıyoruz
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentSort = sortOrder;

            // 3. Sorguyu hazırlıyoruz (Henüz veritabanına gitmedi - AsQueryable)
            var books = _context.Books.Include(b => b.Category).AsQueryable();

            // 4. ARAMA: Kitap adında geçiyor mu?
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            // 5. FİLTRELEME: Belirli bir kategori seçildi mi?
            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryId == categoryId);
            }

            // 6. SIRALAMA: Kullanıcı neye göre sıralamak istedi?[cite: 1]
            books = sortOrder switch
            {
                "name_desc" => books.OrderByDescending(b => b.Title),
                "price_asc" => books.OrderBy(b => b.Price),
                "price_desc" => books.OrderByDescending(b => b.Price),
                "stock_asc" => books.OrderBy(b => b.Stock),
                "stock_desc" => books.OrderByDescending(b => b.Stock),
                _ => books.OrderBy(b => b.Title), // Varsayılan: A-Z[cite: 1]
            };

            return View(books.ToList()); // Ve nihayet veritabanından çekip sayfaya yolluyoruz!
        }

        // KİTAP EKLEME SAYFASI (AÇILIŞ)
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // KİTAP EKLEME (KAYDETME) - Engeller kaldırıldı!
        [HttpPost]
        public IActionResult Add(Book book)
        {
            // ModelState kontrolünü kaldırdık, doğrudan kaydediyoruz
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // KİTAP GÜNCELLEME SAYFASI (AÇILIŞ)
        [HttpGet]
        public IActionResult Update(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // KİTAP GÜNCELLEME (KAYDETME)
        [HttpPost]
        public IActionResult Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // KİTAP SİLME
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}