using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    public class Proposal
    {
        public int Id { get; set; }

        [Display(Name = "Teklif Numarası")]
        public string? ProposalNumber { get; set; }

        [Required(ErrorMessage = "Müşteri adı zorunludur")]
        [Display(Name = "Müşteri Adı/Şirketi")]
        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "Müşteri E-postası")]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Müşteri Adresi")]
        public string? CustomerAddress { get; set; }

        [Display(Name = "Düzenlenme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Display(Name = "Geçerlilik Tarihi")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        // Teklifin durumu
        public ProposalStatus Status { get; set; } = ProposalStatus.Draft;

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // Hesaplanan toplamlar
        public decimal TotalSubtotal { get; set; }
        public decimal TotalVATAmount { get; set; }
        public decimal TotalGrandTotal { get; set; }

        // Teklif kalemleri koleksiyonu
        public ICollection<ProductService> Items { get; set; } = new List<ProductService>();
    }
}