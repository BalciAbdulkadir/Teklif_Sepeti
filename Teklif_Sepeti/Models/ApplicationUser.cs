using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    // Kullanıcı modelimizi IdentityUser'dan türetiyorum ki oturum yönetimi özelliklerini kullanalım
    public class ApplicationUser : IdentityUser
    {
        // PDF ve profil sayfasında kullanılacak şirket bilgileri
        [PersonalData]
        [Display(Name = "Şirket Unvanı")]
        public string? CompanyName { get; set; }

        [PersonalData]
        [Display(Name = "Şirket Adresi")]
        public string? CompanyAddress { get; set; }

        // Vergi bilgileri (PDF'te gösterilecek)
        [PersonalData]
        [Display(Name = "Vergi Dairesi")]
        public string? CompanyTaxOffice { get; set; }

        [PersonalData]
        [Display(Name = "Vergi Numarası")]
        public string? CompanyTaxNumber { get; set; }

        // Ödeme bilgileri
        [PersonalData]
        [Display(Name = "IBAN")]
        public string? CompanyIBAN { get; set; }

        // PDF'te imza alanında kullanılacak yetkili bilgileri
        [PersonalData]
        [Display(Name = "Yetkili Ad Soyad")]
        public string? ContactFullName { get; set; }

        [PersonalData]
        [Display(Name = "Yetkili Unvan")]
        public string? ContactTitle { get; set; }

        // Kullanıcının oluşturduğu teklifler
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}