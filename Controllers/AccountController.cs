using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using libraryApp.Models;

namespace libraryApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Constructor ile Identity servislerini içeri alıyoruz
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ==================== KAYIT OLMA (REGISTER) ====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Yeni kayıt olan kullanıcıya varsayılan olarak "User" rolü veriyoruz
                    await _userManager.AddToRoleAsync(user, "User");

                    // Kayıt işlemi başarılıysa otomatik giriş yaptır ve kitap listesine uçur
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Books");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // ==================== GİRİŞ YAPMA (LOGIN) ====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Books");
                }

                ModelState.AddModelError("", "Hatalı e-posta veya şifre girdiniz.");
            }
            return View(model);
        }

        // ==================== ÇIKIŞ YAPMA (LOGOUT) ====================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Books");
        }

        // ==================== YETKİSİZ ERİŞİM (ACCESS DENIED) ====================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}