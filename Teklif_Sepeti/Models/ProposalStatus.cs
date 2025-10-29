namespace Teklif_Sepeti.Models
{
    public enum ProposalStatus
    {
        Draft,      // Taslak (Üzerinde çalışılıyor)
        Sent,       // Gönderildi (Müşteri bekliyor)
        Accepted,   // Kabul Edildi (Artık faturalanabilir)
        Rejected    // Reddedildi
    }
}
