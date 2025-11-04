using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
      private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
       UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
   _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
    }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
 public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Mevcut þifre alaný zorunludur.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mevcut Þifre")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Yeni þifre alaný zorunludur.")]
            [StringLength(100, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter uzunluðunda olmalýdýr.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Yeni Þifre")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Yeni Þifre (Onay)")]
            [Compare("NewPassword", ErrorMessage = "Yeni þifre ve onay þifresi eþleþmiyor.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
      var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
       return NotFound($"'{_userManager.GetUserId(User)}' ID'li kullanýcý bulunamadý.");
      }

     var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
  {
       return RedirectToPage("./SetPassword");
            }

     return Page();
        }

   public async Task<IActionResult> OnPostAsync()
        {
     if (!ModelState.IsValid)
       {
    return Page();
        }

    var user = await _userManager.GetUserAsync(User);
       if (user == null)
            {
       return NotFound($"'{_userManager.GetUserId(User)}' ID'li kullanýcý bulunamadý.");
    }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
            {
          foreach (var error in changePasswordResult.Errors)
 {
    ModelState.AddModelError(string.Empty, error.Description);
                }
      return Page();
       }

            await _signInManager.RefreshSignInAsync(user);
       _logger.LogInformation("Kullanýcý þifresini baþarýyla deðiþtirdi.");
            StatusMessage = "Þifreniz baþarýyla deðiþtirildi.";

         return RedirectToPage();
     }
    }
}