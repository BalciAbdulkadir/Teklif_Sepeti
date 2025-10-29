using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq; // Bunu ekliyoruz
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Pages
{
    public class CreateProposalModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateProposalModel(ApplicationDbContext context)
        {
            _context = context;
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
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
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

           //Teklif Tarihini ÞÝMDÝ olarak ayarla.
            Proposal.IssueDate = System.DateTime.Now;
            

            // 6. DEPOYA KALDIR
            _context.Proposals.Add(Proposal);
            await _context.SaveChangesAsync();

            // 7. MÝSAFÝRÝ UÐURLA
            return RedirectToPage("/Index");
        }
    }
}