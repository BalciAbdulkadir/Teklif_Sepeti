using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    public class ProductService
    {
    public int Id { get; set; }

   [Required(ErrorMessage = "Kalem açıklaması zorunludur")]
   public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Geçerli bir birim fiyat giriniz")]
        public decimal UnitPrice { get; set; }

        [Range(0, 100, ErrorMessage = "KDV oranı 0-100 arasında olmalıdır")]
        public decimal VATRate { get; set; }

        public decimal CalculatedSubtotal
        {
  get { return Quantity * UnitPrice; }
            set { }
    }

        public decimal CalculatedVAT
        {
       get { return CalculatedSubtotal * (VATRate / 100m); }
    set { }
        }

        public decimal CalculatedTotal
        {
       get { return CalculatedSubtotal + CalculatedVAT; }
  set { }
        }

      public int ProposalId { get; set; }
      public Proposal? Proposal { get; set; }
    }
}