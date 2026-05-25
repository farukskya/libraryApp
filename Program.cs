using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using libraryApp.Data;

var builder = WebApplication.CreateBuilder(args);

// ==================== 1. VERÝTABANI BAÐLANTISI ====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== 2. IDENTITY SERVÝS AYARLARI ====================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Okul bilgisayarlarýnda ve testlerde þifre zorluðu yaþanmamasý için kurallarý esnetiyoruz
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ==================== 3. COOKIE VE KESÝN YÖNLENDÝRME AYARLARI ====================
// Burasý çok kritik! Tarayýcýnýn 'User' rolüyle yetkisiz bir iþleme (Silme/Ekleme vb.) 
// týklandýðýnda hata vermeden direkt AccessDenied sayfasýna fýrlatmasýný saðlar.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";               // Giriþ yapmayanlar buraya uçacak
    options.AccessDeniedPath = "/Account/AccessDenied";   // Yetkisiz silme/ekleme yapanlar buraya uçacak
    options.LogoutPath = "/Account/Logout";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==================== 4. OTOMATÝK ROL OLUÞTURMA MEKANÝZMASI ====================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ==================== 5. GÜVENLÝK BORU HATTI (MIDDLEWARE OYUNU) ====================
// Sýralama hayati önem taþýr: Önce Kimlik Doðrulanýr, Sonra Yetki Kontrol Edilir!
app.UseAuthentication(); // Kimliði Doðrula (Ben kimim?)
app.UseAuthorization();  // Yetki Kontrolü Yap (Buraya girmeye iznim var mý?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();