using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teklif_Sepeti.Models
{
    public class Proposal
    {
      public int Id { get; set; }

 // Her teklifin benzersiz bir numarası olsun
        [Display(Name = "Teklif Numarası")]
        public string? ProposalNumber { get; set; }

        // Müşteri bilgileri - bunlar PDF'te başlıkta görünecek
      [Required(ErrorMessage = "Müşterinin adını girmelisin")]
   [Display(Name = "Müşteri Adı/Şirketi")]
        public string CustomerName { get; set; }

      [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girmelisin")]
        [Display(Name = "Müşteri E-postası")]
        public string? CustomerEmail { get; set; }

      [Display(Name = "Müşteri Adresi")]
   public string? CustomerAddress { get; set; }

 // Teklif tarihleri - geçerlilik kontrolü için kullanacağım
        [Display(Name = "Düzenlenme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

   [Display(Name = "Geçerlilik Tarihi")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        // Teklifin durumu - iş akışını takip etmek için
        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;

     [Display(Name = "Notlar")]
      public string? Notes { get; set; }

      // Hesaplanan toplamlar - bunları kalemlerden otomatik hesaplayacağım
        public decimal TotalSubtotal { get; set; }
    public decimal TotalVATAmount { get; set; }
        public decimal TotalGrandTotal { get; set; }

        // Teklif kalemleri - null kontrolü yapmamak için boş liste ile başlat
        public ICollection<ProductService> Items { get; set; } = new List<ProductService>();

        // Teklifi oluşturan kullanıcının bilgileri
        [Required]
 public string ApplicationUserId { get; set; }
        
 [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}