using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    public class Proposal
    {
        public int Id { get; set; }

        // --- MÜŞTERİ VE TEKLİF BİLGİLERİ ---

        [Display(Name = "Teklif Numarası")]
        public string? ProposalNumber { get; set; } // ARTIK BOŞ OLABİLİR

        [Required(ErrorMessage = "Müşteri Adı alanı zorunludur.")] // Hata mesajı eklemek iyidir
        [Display(Name = "Müşteri Adı/Şirketi")]
        public string CustomerName { get; set; }

        [Display(Name = "Müşteri E-Postası")]
        [EmailAddress]
        public string? CustomerEmail { get; set; } // ARTIK BOŞ OLABİLİR

        // Müşteri Adresini de ekleyelim, çünkü formda var.
        [Display(Name = "Müşteri Adresi")]
        public string? CustomerAddress { get; set; } // ARTIK BOŞ OLABİLİR

        [Display(Name = "Düzenlenme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Geçerlilik Bitiş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        // --- DURUM VE NOTLAR ---

        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;

        [Display(Name = "Ek Notlar")]
        public string? Notes { get; set; } // ARTIK BOŞ OLABİLİR

        // --- HESAPLANMIŞ TOPLAMLAR ---
        // Bunları formdan beklemiyoruz, arka planda hesaplayacağız
        public decimal TotalSubtotal { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalGrandTotal { get; set; }

        // Bu önemli: İlişkiyi boş bir liste ile başlatmak, null hatalarını önler.
        public ICollection<ProductService> Items { get; set; } = new List<ProductService>();
    }

    
}