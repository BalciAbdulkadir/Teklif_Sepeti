using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;
using Teklif_Sepeti.Documents;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Teklif_Sepeti.Pages
{
    [Authorize]
    public class ProposalDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProposalDetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // BU ÇOK ÖNEMLÝ
        // [BindProperty] DEÐÝL. Bu özellik sadece sayfa yüklendiðinde (GET) doldurulacak.
        public Proposal Proposal { get; set; }

        // "Detaylar" butonuna basýnca (GET) çalýþýr
        public async Task<IActionResult> OnGetAsync(int? id)
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

            // Sadece bu kullanýcýya aitse ve ID eþleþiyorsa getir
            Proposal = await _context.Proposals
                                     .Include(p => p.Items)
                                     .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == user.Id);

            if (Proposal == null)
            {
                // Teklif yoksa veya baþkasýna aitse, 404 döndür.
                // Sayfayý ASLA yüklemeye çalýþma. NullReference hatasý burada engellendi.
                return NotFound();
            }

            // Teklif bulundu. Sayfayý yükle.
            return Page();
        }

        // "PDF Ýndir" butonu (POST) çalýþýr
        public async Task<IActionResult> OnPostDownloadPdfAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var proposal = await _context.Proposals
                                         .Include(p => p.Items)
                                         .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == user.Id);

            if (proposal == null)
            {
                return NotFound(); // Baþkasýnýn teklifini indiremesin
            }

            // Bu metot ASLA 'Page()' döndürmez.
            // Sadece PDF dosyasýný oluþturur ve indirir.
            var document = new ProposalDocument(proposal, user);
            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Teklif-{proposal.ProposalNumber ?? proposal.Id.ToString()}.pdf");
        }
    }
}