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
    [Authorize]
    public class CreateProposalModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

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
                Items = new List<ProductService>(),
                DiscountValue = 0
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Proposal.ApplicationUserId");
            ModelState.Remove("Proposal.ApplicationUser");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            Proposal.ApplicationUserId = user.Id;

            // --- HESAPLAMA MANTIÐI ---

            decimal totalSubtotal = 0;
            decimal totalVatAmount = 0;

            // 1. Önce satýrlarý ve KDV'leri hesapla
            if (Proposal.Items != null && Proposal.Items.Any())
            {
                foreach (var item in Proposal.Items)
                {
                    var rowSubtotal = item.Quantity * item.UnitPrice;
                    var rowVat = rowSubtotal * (item.VATRate / 100);

                    item.CalculatedSubtotal = rowSubtotal;
                    item.CalculatedVAT = rowVat;
                    item.CalculatedTotal = rowSubtotal + rowVat;

                    totalSubtotal += rowSubtotal;
                    totalVatAmount += rowVat;
                }
            }

            // 2. Ýskontoyu hesapla
            decimal totalDiscountAmount = 0;
            if (Proposal.DiscountType == DiscountType.Percentage)
            {
                totalDiscountAmount = totalSubtotal * (Proposal.DiscountValue / 100);
            }
            else // FixedAmount
            {
                totalDiscountAmount = Proposal.DiscountValue;
            }

            // 3. Net ve Genel Toplamlarý hesapla
            decimal totalNetTotal = totalSubtotal - totalDiscountAmount;
            decimal totalGrandTotal = totalNetTotal + totalVatAmount;

            // 4. Tüm hesaplanan deðerleri Proposal nesnesine ata
            Proposal.TotalSubtotal = totalSubtotal;
            Proposal.TotalDiscountAmount = totalDiscountAmount;
            Proposal.TotalNetTotal = totalNetTotal;
            Proposal.TotalVATAmount = totalVatAmount;
            Proposal.TotalGrandTotal = totalGrandTotal;

            // --- HESAPLAMA MANTIÐI BÝTTÝ ---

            Proposal.ProposalNumber = $"TKLF-{System.DateTime.Now:yyyyMMdd-HHmm}";
            Proposal.IssueDate = System.DateTime.Now;

            _context.Proposals.Add(Proposal);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ProposalList");
        }
    }
}