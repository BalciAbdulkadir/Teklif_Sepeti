using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Include ve veritabaný iþlemleri için gerekli
using QuestPDF.Fluent; // PDF üretimi için QuestPDF
using System.Threading.Tasks;
using Teklif_Sepeti.Data; // Veritabaný context'i için
using Teklif_Sepeti.Models; // Proposal ve ProductService modelleri için
using Teklif_Sepeti.Documents; // ProposalDocument sýnýfý için

namespace Teklif_Sepeti.Pages
{
    public class ProposalDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProposalDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sayfada gösterilecek olan teklifi tutacak property
        public Proposal Proposal { get; set; }

        // Sayfa ilk yüklendiðinde çalýþacak metot (GET isteði)
        // Adres çubuðundan gelen 'id' parametresini alýr
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Eðer ID gelmezse 404 hatasý döndür
            }

            // Veritabanýndan ilgili teklifi bul
            // Include(p => p.Items) ile teklife baðlý kalemleri de yükle
            Proposal = await _context.Proposals
                                     .Include(p => p.Items) // Ýliþkili ProductService'leri de getir
                                     .FirstOrDefaultAsync(p => p.Id == id); // ID'ye göre teklifi bul

            if (Proposal == null)
            {
                return NotFound(); // Eðer o ID ile teklif bulunamazsa 404 hatasý döndür
            }

            return Page(); // Her þey yolundaysa, Proposal nesnesi dolu olarak sayfayý döndür
        }

        // PDF Ýndirme butonu týklandýðýnda çalýþacak metot (POST isteði)
        // Formdan gönderilen 'id' parametresini alýr
        public async Task<IActionResult> OnPostDownloadPdfAsync(int id)
        {
            // Güvenlik için teklifi veritabanýndan tekrar çek
            var proposal = await _context.Proposals
                                         .Include(p => p.Items) // Kalemleri de dahil et
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (proposal == null)
            {
                return NotFound(); // Teklif bulunamazsa hata ver
            }

            // QuestPDF þablonumuzu (ProposalDocument) oluþtur ve teklif verisini içine ver
            var document = new ProposalDocument(proposal);

            // PDF'i hafýzada byte dizisi olarak oluþtur
            byte[] pdfBytes = document.GeneratePdf();

            // Oluþturulan PDF'i kullanýcýya dosya olarak indir
            // File() metodu FileContentResult döndürür
            // Parametreler: byte[] içerik, MIME tipi, dosya adý
            return File(pdfBytes, "application/pdf", $"Teklif-{proposal.ProposalNumber ?? proposal.Id.ToString()}.pdf");
        }
    }
}