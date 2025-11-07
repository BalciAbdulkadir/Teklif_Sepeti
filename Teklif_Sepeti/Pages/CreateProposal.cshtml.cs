using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Teklif_Sepeti.Pages
{
    [Authorize] // Sadece giriþ yapanlar teklif oluþturabilsin
    public class CreateProposalModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor güncellendi
        public CreateProposalModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Proposal Proposal { get; set; }

        public void OnGet()
        {
            Proposal = new Proposal
            {
                IssueDate = System.DateTime.Now,
                ExpiryDate = System.DateTime.Now.AddDays(15),
                Items = new List<ProductService>()
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // --- HATA DÜZELTMESÝ BURADA ---
            // Model'e diyoruz ki, bu iki alaný formdan bekleme,
            // bunlarý biz elle atayacaðýz, sen doðrulama dýþý býrak.
            ModelState.Remove("Proposal.ApplicationUserId");
            ModelState.Remove("Proposal.ApplicationUser");
            // --- HATA DÜZELTMESÝ BÝTTÝ ---

            if (!ModelState.IsValid)
            {
                // Artýk buraya (boþ bir müþteri adý yollanmadýkça) düþmemeli
                return Page();
            }

            // --- KULLANICIYI BUL VE TEKLÝFE ATA ---
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            Proposal.ApplicationUserId = user.Id;
            // --- KULLANICI ATAMA BÝTTÝ ---


            // ...(Hesaplama kodlarý)...
            decimal totalSubtotal = 0;
            decimal totalVatAmount = 0;

            if (Proposal.Items != null && Proposal.Items.Any())
            {
                foreach (var item in Proposal.Items)
                {
                    var rowSubtotal = item.Quantity * item.UnitPrice;
                    var rowVat = rowSubtotal * (item.VATRate / 100);
                    var rowTotal = rowSubtotal + rowVat;

                    item.CalculatedSubtotal = rowSubtotal;
                    item.CalculatedVAT = rowVat;
                    item.CalculatedTotal = rowTotal;

                    totalSubtotal += rowSubtotal;
                    totalVatAmount += rowVat;
                }
            }

            Proposal.TotalSubtotal = totalSubtotal;
            Proposal.TotalVATAmount = totalVatAmount;
            Proposal.TotalGrandTotal = totalSubtotal + totalVatAmount;
            Proposal.ProposalNumber = $"TKLF-{System.DateTime.Now:yyyyMMdd-HHmm}";
            Proposal.IssueDate = System.DateTime.Now;

            // 6. DEPOYA KALDIR
            _context.Proposals.Add(Proposal);
            await _context.SaveChangesAsync();

            // 7. MÝSAFÝRÝ UÐURLA
            return RedirectToPage("/ProposalList"); // Liste sayfasýna yönlendirelim
        }
    }
}