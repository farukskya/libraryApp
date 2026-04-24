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
        public IActionResult Index()
        {
            // Kategorileriyle birlikte kitapları çekiyoruz
            var books = _context.Books.Include(b => b.Category).ToList();
            return View(books);
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