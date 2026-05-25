using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using libraryApp.Data;
using libraryApp.Models;
using Microsoft.AspNetCore.Authorization;
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

        // ==================== 1. LİSTELEME (INDEX) ====================
        [AllowAnonymous] // Herkese açık (Anonim kullanıcı görebilir)
        public IActionResult Index(string searchString, int? categoryId, string sortOrder)
        {
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParam"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["StockSortParam"] = sortOrder == "Stock" ? "stock_desc" : "Stock";

            var books = _context.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryId == categoryId);
            }

            switch (sortOrder)
            {
                case "name_desc": books = books.OrderByDescending(b => b.Title); break;
                case "Price": books = books.OrderBy(b => b.Price); break;
                case "price_desc": books = books.OrderByDescending(b => b.Price); break;
                case "Stock": books = books.OrderBy(b => b.Stock); break;
                case "stock_desc": books = books.OrderByDescending(b => b.Stock); break;
                default: books = books.OrderBy(b => b.Title); break;
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(books.ToList());
        }

        // ==================== 2. DETAY SAYFASI (DETAILS) ====================
        [Authorize(Roles = "Admin,User")] // Anonim basarsa LOGIN'e uçar, User/Admin görebilir
        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // ==================== 3. KİTAP EKLEME (ADD - GET) ====================
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // ==================== 4. KİTAP EKLEME (ADD - POST) ====================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add(Book book)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ==================== 5. GÜNCELLEME (UPDATE - GET) ====================
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ==================== 6. GÜNCELLEME (UPDATE - POST) ====================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Book book)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Entry(book).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ==================== 7. KİTAP SİLME (DELETE - POST) ====================
        // HOCANIN İSTEDİĞİ GİBİ SADECE POST! 
        // Giriş yapmayan veya yetkisiz biri buraya sızmaya çalışırsa sistem doğrudan engeller.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}