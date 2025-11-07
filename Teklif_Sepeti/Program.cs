using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;  
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;

var builder = WebApplication.CreateBuilder(args);

// Temel servisleri ekliyorum
builder.Services.AddRazorPages();

// SQLite veritabaný baðlantýsýný kuruyorum
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity ayarlarýný yapýyorum - e-posta onayý istemiyorum çünkü test ortamýndayýz
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// PDF kütüphanesinin lisansýný ayarlýyorum
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Hata sayfalarýný ayarlýyorum
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware'leri sýralýyorum
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Önce kimlik doðrulama, sonra yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();