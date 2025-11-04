using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Teklif_Sepeti.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "Şirket Unvanı")]
        public string? CompanyName { get; set; }

        [PersonalData]
        [Display(Name = "Şirket Adresi")]
        public string? CompanyAddress { get; set; }

        [PersonalData]
        [Display(Name = "Vergi Dairesi")]
        public string? CompanyTaxOffice { get; set; }

        [PersonalData]
        [Display(Name = "Vergi Numarası")]
        public string? CompanyTaxNumber { get; set; }

        [PersonalData]
        [Display(Name = "IBAN")]
        public string? CompanyIBAN { get; set; }

        [PersonalData]
        [Display(Name = "Yetkili Ad Soyad")]
        public string? ContactFullName { get; set; }

        [PersonalData]
        [Display(Name = "Yetkili Unvan")]
        public string? ContactTitle { get; set; }
    }
}