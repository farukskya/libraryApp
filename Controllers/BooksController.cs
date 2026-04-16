using Microsoft.AspNetCore.Mvc;
using libraryApp.Data;
using libraryApp.Models;
using System.Linq;

namespace libraryApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // 1) Kitapları Listeleme
        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        // 2) Kitap Ekleme Sayfası [GET]
        public IActionResult Add()
        {
            return View();
        }

        // 3) Kitap Ekleme İşlemi [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Book newBook)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(newBook);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kitap başarıyla eklendi!";
                return RedirectToAction("Index");
            }
            return View(newBook);
        }

        // 4) Güncelleme Sayfası [GET]
        public IActionResult Update(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // 5) Güncelleme İşlemi [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Book updatedBook)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(updatedBook);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kitap başarıyla güncellendi!";
                return RedirectToAction("Index");
            }
            return View(updatedBook);
        }

        // 6) Kitap Silme İşlemi
        public IActionResult Remove(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kitap başarıyla silindi!";
            }
            return RedirectToAction("Index");
        }
    }
}