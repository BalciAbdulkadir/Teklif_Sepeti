using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options; // IOptions için
using System.Net;
using System.Net.Mail; // SmtpClient ve MailMessage için

namespace Teklif_Sepeti.Services
{
    // Bu artık "gerçek" e-posta göndericisidir.
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        // Constructor (Yapıcı Metot)
        // Program.cs'te kaydettiğimiz SmtpSettings ayarlarını
        // IOptions<T> aracılığıyla buraya enjekte ediyoruz.
        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        // Identity kütüphanesinin e-posta göndermek için çağıracağı metot
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // 1. E-posta Mesajını Oluştur
            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email); // Alıcıyı ekle

            // 2. SMTP İstemcisini (Client) Oluştur
            using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                // Güvenli bağlantı (SSL/TLS)
                client.EnableSsl = true;

                // Kimlik Bilgileri (appsettings.json'dan gelen)
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                try
                {
                    // 3. E-postayı Gönder
                    await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    // Bir hata olursa, konsola yazdır (geliştirme için)
                    // Normalde buraya daha gelişmiş bir loglama mekanizması kurulur.
                    Console.WriteLine("E-POSTA GÖNDERİLEMEDİ:");
                    Console.WriteLine(ex.Message);
                    // Hatayı fırlat ki fark edilsin
                    throw;
                }
            }
        }
    }
}