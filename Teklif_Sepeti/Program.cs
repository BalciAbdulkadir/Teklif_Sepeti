using Microsoft.EntityFrameworkCore;
using Teklif_Sepeti.Data;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Teklif_Sepeti.Models; // <-- EKLENDÝ (ApplicationUser için)
using Microsoft.AspNetCore.Identity.UI.Services; // <-- EKLENDÝ (Email için)
using Teklif_Sepeti.Services; // <-- EKLENDÝ (Email için)


var builder = WebApplication.CreateBuilder(args);

// SQLite Desteði (ApplicationDbContext'i kullan)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- ANA AMELÝYAT BURADA ---
// AddDefaultIdentity'yi 'IdentityUser'dan 'ApplicationUser'a çeviriyoruz.
// 'false' olan sigortayý 'true' yapýyoruz.
// 'Teklif_SepetiContext' yerine 'ApplicationDbContext'i gösteriyoruz.
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // <-- 'false' idi 'true' oldu
    options.SignIn.RequireConfirmedEmail = true;   // <-- YENÝ EKLENDÝ
})
    .AddEntityFrameworkStores<ApplicationDbContext>(); // <-- 'Teklif_SepetiContext' idi
// --- AMELÝYAT BÝTTÝ ---

// Email Servislerini ekliyoruz (Bunlar "temiz" repoda yoktu)
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddRazorPages();
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();