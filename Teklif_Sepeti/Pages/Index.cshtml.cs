using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Bunu eklemen çok önemli
using System.Collections.Generic;
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Pages
{
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        private readonly ApplicationDbContext _context = context;

        // CS8618: Null atanamaz özellik 'Proposals', oluþturucudan çýkýþ yaparken null olmayan bir deðer içermelidir.
        public IList<Proposal> Proposals { get; set; } = new List<Proposal>();

        // Sayfa yüklendiðinde bu metot otomatik çalýþýr
        public async Task OnGetAsync()
        {
            // Veritabanýndaki Proposals tablosuna git,
            // Hepsini (ToListAsync) al ve
            // Proposals listemize doldur.
            Proposals = await _context.Proposals.ToListAsync();
        }
    }
}