using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity; // Bunu ekliyoruz
using Teklif_Sepeti.Models; // ApplicationUser için bunu ekliyoruz
using System.Threading.Tasks; // Task için
using Microsoft.AspNetCore.Authorization; // Bunu ekliyoruz

namespace Teklif_Sepeti.Pages
{
    [Authorize] // Bu sayfa sadece giriþ yapanlara
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // HTML tarafýnda kullanmak için bir property
        public ApplicationUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // O an giriþ yapmýþ olan kullanýcýyý ID'sine göre bul
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Eðer bir sebepten kullanýcý bulunamazsa (normalde olmamalý)
                // Hata yerine giriþ sayfasýna yönlendirelim
                return Challenge();
            }

            // Bulduðumuz kullanýcýyý HTML'de kullanabilmek için property'mize ata
            CurrentUser = user;
            return Page();
        }
    }
}