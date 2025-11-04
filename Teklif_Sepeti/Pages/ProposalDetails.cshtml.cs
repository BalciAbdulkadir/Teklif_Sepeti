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

namespace Teklif_Sepeti.Pages
{
    [Authorize] //  Bu sayfayý sadece giriþ yapanlar görebilsin
    public class ProposalDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; //Kullanýcý yöneticisini ekle

        // Constructor'a UserManager'ý da ekle
        public ProposalDetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; //  UserManager'ý ata
        }

        public Proposal Proposal { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Proposal = await _context.Proposals
                                     .Include(p => p.Items)
                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (Proposal == null)
            {
                return NotFound();
            }

            return Page();
        }

        // PDF Ýndirme butonu týklandýðýnda çalýþacak metot
        public async Task<IActionResult> OnPostDownloadPdfAsync(int id)
        {
            // 1. Adým: Teklifi veritabanýndan çek (Kalemleriyle birlikte)
            var proposal = await _context.Proposals
                                         .Include(p => p.Items)
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (proposal == null)
            {
                return NotFound();
            }

            //  O an giriþ yapmýþ olan kullanýcýyý bul
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Giriþ yapmamýþ biri bir þekilde buraya ulaþtýysa (çok zor)
                return Challenge(); // Onu login sayfasýna yönlendir
            }

            // 2. Adým: PDF Þablonunu oluþtur
            //  Constructor'a artýk HEM teklifi HEM de kullanýcýyý ver
            var document = new ProposalDocument(proposal, user);

            // 3. Adým: PDF'i hafýzada oluþtur
            byte[] pdfBytes = document.GeneratePdf();

            // 4. Adým: Dosyayý kullanýcýya indir
            return File(pdfBytes, "application/pdf", $"Teklif-{proposal.ProposalNumber ?? proposal.Id.ToString()}.pdf");
        }
    }
}