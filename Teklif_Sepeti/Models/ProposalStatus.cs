using System.ComponentModel.DataAnnotations; 

namespace Teklif_Sepeti.Models
{
    public enum ProposalStatus
    {
        [Display(Name = "Taslak")] 
        Draft,

        [Display(Name = "Gönderildi")] 
        Sent,

        [Display(Name = "Kabul Edildi")] 
        Accepted,

        [Display(Name = "Reddedildi")] 
        Rejected
    }
}