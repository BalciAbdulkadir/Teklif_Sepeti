namespace Teklif_Sepeti.Services
{
    // appsettings.json dosyasındaki SMTP ayarlarımızı
    // temsil edecek olan C# sınıfı.
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}