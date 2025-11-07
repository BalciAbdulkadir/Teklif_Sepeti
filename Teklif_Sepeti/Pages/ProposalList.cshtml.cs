using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;
using Microsoft.AspNetCore.Identity;

namespace Teklif_Sepeti.Pages
{
    [Authorize]
    public class ProposalListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProposalListModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Proposal> Proposals { get; set; } = new List<Proposal>();

        [TempData]
        public string StatusMessage { get; set; }

        // SAYFAYI YÜKLER (GET)
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Proposals = await _context.Proposals
                                        .Where(p => p.ApplicationUserId == user.Id)
                                        .OrderByDescending(p => p.IssueDate)
                                        .ToListAsync();
            }
        }

        // "SÝL" BUTONU ÇALIÞINCA (POST)
        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Sadece bu kullanýcýya aitse ve ID eþleþiyorsa bul
            var proposal = await _context.Proposals
                                 .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == user.Id);

            if (proposal != null)
            {
                // Ýliþkili kalemleri (ProductService) de bul
                var items = await _context.ProductServices
                                          .Where(item => item.ProposalId == proposal.Id)
                                          .ToListAsync();

                // Önce kalemleri sil
                _context.ProductServices.RemoveRange(items);
                // Sonra teklifin kendisini sil
                _context.Proposals.Remove(proposal);

                await _context.SaveChangesAsync();
                StatusMessage = "Teklif baþarýyla silindi.";
            }
            else
            {
                StatusMessage = "Hata: Teklif bulunamadý veya silme yetkiniz yok.";
            }

            // Sayfayý yeniden yükle
            return RedirectToPage();
        }
    }
}