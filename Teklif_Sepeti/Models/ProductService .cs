using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    public class ProductService
    {
        public int Id { get; set; }

    // Zorunlu alanlar - validasyon mesajlarını kullanıcı dostu yazdım
    [Required(ErrorMessage = "Bir açıklama yazmalısın")]
        [Display(Name = "Açıklama")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalı")]
  [Display(Name = "Miktar")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat sıfırdan büyük olmalı")]
        [Display(Name = "Birim Fiyat")]
        public decimal UnitPrice { get; set; }

      [Range(0, 100, ErrorMessage = "KDV oranı 0-100 arasında olmalı")]
      [Display(Name = "KDV (%)")]
        public decimal VATRate { get; set; }

    // Hesaplanan değerler - bunları otomatik hesaplıyorum
        [Display(Name = "Ara Toplam")]
        public decimal CalculatedSubtotal
        {
   get { return Quantity * UnitPrice; }
     set { }
        }

        [Display(Name = "KDV Tutarı")]
        public decimal CalculatedVAT
        {
            get { return CalculatedSubtotal * (VATRate / 100m); }
  set { }
        }

[Display(Name = "Toplam")]
        public decimal CalculatedTotal
        {
      get { return CalculatedSubtotal + CalculatedVAT; }
        set { }
 }

        // İlişki - Bu kalem hangi teklife ait
        public int ProposalId { get; set; }
        public Proposal? Proposal { get; set; }
    }
}