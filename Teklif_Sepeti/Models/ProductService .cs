using System.ComponentModel.DataAnnotations; // Bunu ekle

namespace Teklif_Sepeti.Models
{
    public class ProductService
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Satır açıklaması boş bırakılamaz.")] // Kuralı netleştir
        public string Name { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VATRate { get; set; }

        // --- HESAPLANMIŞ ALANLAR ---
        public decimal CalculatedSubtotal { get; set; }
        public decimal CalculatedVAT { get; set; }
        public decimal CalculatedTotal { get; set; }


        public int ProposalId { get; set; }

        // --- MAYINI TEMİZLEDİK ---
        // Bu, bir "navigation property"dir. Model bağlayıcının
        // bunu formdan beklemesine gerek yok. "Null olabilir" (? işareti) 
        // diyerek fedainin çenesini kapatıyoruz.
        // Entity Framework (veritabanı) bunu ProposalId üzerinden kendi halledecek.
        public Proposal? Proposal { get; set; }
    }
}