using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Teklif_Sepeti.Models; 


namespace Teklif_Sepeti.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        
        public class InputModel
        {
            [Phone]
            [Display(Name = "Telefon Numarası")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Şirket Unvanı")]
            public string CompanyName { get; set; }

            [Display(Name = "Şirket Adresi")]
            public string CompanyAddress { get; set; }

            [Display(Name = "Vergi Dairesi")]
            public string CompanyTaxOffice { get; set; }

            [Display(Name = "Vergi Numarası")]
            public string CompanyTaxNumber { get; set; }

            [Display(Name = "IBAN")]
            public string CompanyIBAN { get; set; }

            [Display(Name = "Yetkili Adı Soyadı")]
            public string ContactFullName { get; set; }

            [Display(Name = "Yetkili Unvanı")]
            public string ContactTitle { get; set; }
        }

        // Sayfa yüklenirken veritabanındaki bilgileri forma doldurur
        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                
                // Veritabanından (user nesnesinden) verileri çekip forma dolduruyoruz
                CompanyName = user.CompanyName,
                CompanyAddress = user.CompanyAddress,
                CompanyTaxOffice = user.CompanyTaxOffice,
                CompanyTaxNumber = user.CompanyTaxNumber,
                CompanyIBAN = user.CompanyIBAN,
                ContactFullName = user.ContactFullName,
                ContactTitle = user.ContactTitle
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Telefon numarası ayarlanırken beklenmedik hata.";
                    return RedirectToPage();
                }
            }

            
            // Formdan (Input) gelen verileri alıp 'user' nesnesine işliyoruz
            bool dataChanged = false;

            if (Input.CompanyName != user.CompanyName)
            {
                user.CompanyName = Input.CompanyName;
                dataChanged = true;
            }
            if (Input.CompanyAddress != user.CompanyAddress)
            {
                user.CompanyAddress = Input.CompanyAddress;
                dataChanged = true;
            }
            if (Input.CompanyTaxOffice != user.CompanyTaxOffice)
            {
                user.CompanyTaxOffice = Input.CompanyTaxOffice;
                dataChanged = true;
            }
            if (Input.CompanyTaxNumber != user.CompanyTaxNumber)
            {
                user.CompanyTaxNumber = Input.CompanyTaxNumber;
                dataChanged = true;
            }
            if (Input.CompanyIBAN != user.CompanyIBAN)
            {
                user.CompanyIBAN = Input.CompanyIBAN;
                dataChanged = true;
            }
            if (Input.ContactFullName != user.ContactFullName)
            {
                user.ContactFullName = Input.ContactFullName;
                dataChanged = true;
            }
            if (Input.ContactTitle != user.ContactTitle)
            {
                user.ContactTitle = Input.ContactTitle;
                dataChanged = true;
            }

            // Sadece değişiklik varsa veritabanını güncelle
            if (dataChanged)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Profil güncellenirken beklenmedik hata.";
                    return RedirectToPage();
                }
            }

          

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profiliniz güncellendi";
            return RedirectToPage();
        }
    }
}