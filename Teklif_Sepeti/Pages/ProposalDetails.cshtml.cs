using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Include ve veritaban� i�lemleri i�in gerekli
using QuestPDF.Fluent; // PDF �retimi i�in QuestPDF
using System.Threading.Tasks;
using Teklif_Sepeti.Data; // Veritaban� context'i i�in
using Teklif_Sepeti.Models; // Proposal ve ProductService modelleri i�in
using Teklif_Sepeti.Documents; // ProposalDocument s�n�f� i�in

namespace Teklif_Sepeti.Pages
{
    public class ProposalDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProposalDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sayfada g�sterilecek olan teklifi tutacak property
        public Proposal Proposal { get; set; }

        // Sayfa ilk y�klendi�inde �al��acak metot (GET iste�i)
        // Adres �ubu�undan gelen 'id' parametresini al�r
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // E�er ID gelmezse 404 hatas� d�nd�r
            }

            // Veritaban�ndan ilgili teklifi bul
            // Include(p => p.Items) ile teklife ba�l� kalemleri de y�kle
            Proposal = await _context.Proposals
                                     .Include(p => p.Items) // �li�kili ProductService'leri de getir
                                     .FirstOrDefaultAsync(p => p.Id == id); // ID'ye g�re teklifi bul

            if (Proposal == null)
            {
                return NotFound(); // E�er o ID ile teklif bulunamazsa 404 hatas� d�nd�r
            }

            return Page(); // Her �ey yolundaysa, Proposal nesnesi dolu olarak sayfay� d�nd�r
        }

        // PDF �ndirme butonu t�kland���nda �al��acak metot (POST iste�i)
        // Formdan g�nderilen 'id' parametresini al�r
        public async Task<IActionResult> OnPostDownloadPdfAsync(int id)
        {
            // G�venlik i�in teklifi veritaban�ndan tekrar �ek
            var proposal = await _context.Proposals
                                         .Include(p => p.Items) // Kalemleri de dahil et
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (proposal == null)
            {
                return NotFound(); // Teklif bulunamazsa hata ver
            }

            // QuestPDF �ablonumuzu (ProposalDocument) olu�tur ve teklif verisini i�ine ver
            var document = new ProposalDocument(proposal);

            // PDF'i haf�zada byte dizisi olarak olu�tur
            byte[] pdfBytes = document.GeneratePdf();

            // Olu�turulan PDF'i kullan�c�ya dosya olarak indir
            // File() metodu FileContentResult d�nd�r�r
            // Parametreler: byte[] i�erik, MIME tipi, dosya ad�
            return File(pdfBytes, "application/pdf", $"Teklif-{proposal.ProposalNumber ?? proposal.Id.ToString()}.pdf");
        }
    }
}