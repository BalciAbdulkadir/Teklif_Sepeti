using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Bunu eklemen �ok �nemli
using System.Collections.Generic;
using System.Threading.Tasks;
using Teklif_Sepeti.Data;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Pages
{
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        private readonly ApplicationDbContext _context = context;

        // CS8618: Null atanamaz �zellik 'Proposals', olu�turucudan ��k�� yaparken null olmayan bir de�er i�ermelidir.
        public IList<Proposal> Proposals { get; set; } = new List<Proposal>();

        // Sayfa y�klendi�inde bu metot otomatik �al���r
        public async Task OnGetAsync()
        {
            // Veritaban�ndaki Proposals tablosuna git,
            // Hepsini (ToListAsync) al ve
            // Proposals listemize doldur.
            Proposals = await _context.Proposals.ToListAsync();
        }
    }
}