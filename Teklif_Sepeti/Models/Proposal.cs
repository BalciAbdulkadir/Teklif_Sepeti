using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teklif_Sepeti.Models
{
    // Yüzde veya Sabit Tutar için bir enum oluşturduk
    public enum DiscountType
    {
        [Display(Name = "Yüzde (%)")]
        Percentage,
        [Display(Name = "Sabit Tutar (₺)")]
        FixedAmount
    }

    public class Proposal
    {
        public int Id { get; set; }

        // --- MÜŞTERİ VE TEKLİF BİLGİLERİ ---
        [Display(Name = "Teklif Numarası")]
        public string? ProposalNumber { get; set; }

        [Required(ErrorMessage = "Müşterinin adını girmelisin")]
        [Display(Name = "Müşteri Adı/Şirketi")]
        public string CustomerName { get; set; }

        [Display(Name = "Müşteri E-Postası")]
        [EmailAddress]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Müşteri Adresi")]
        public string? CustomerAddress { get; set; }

        [Display(Name = "Düzenlenme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Geçerlilik Tarihi")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // --- İSKONTO ALANLARI ---
        [Display(Name = "İskonto Tipi")]
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        [Display(Name = "İskonto Değeri")]
        public decimal DiscountValue { get; set; } = 0;

        // --- HESAPLANMIŞ TOPLAMLAR ---
        public decimal TotalSubtotal { get; set; } // Ara Toplam (İskontosuz)
        public decimal TotalDiscountAmount { get; set; } // Hesaplanan İskonto Tutarı (BU EKSİKTİ)
        public decimal TotalNetTotal { get; set; } // Net Toplam (Ara Toplam - İskonto) (BU EKSİKTİ)
        public decimal TotalVATAmount { get; set; } // Toplam KDV
        public decimal TotalGrandTotal { get; set; } // Genel Toplam (Net Toplam + KDV)


        public ICollection<ProductService> Items { get; set; } = new List<ProductService>();

        // Teklifi oluşturan kullanıcının bilgileri
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}