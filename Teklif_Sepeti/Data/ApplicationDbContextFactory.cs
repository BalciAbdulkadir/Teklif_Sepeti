using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Teklif_Sepeti.Data
{
    // Bu sınıf, EF Core'un migration sırasında DbContext'i oluşturmasına yardımcı olur.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Konfigürasyonu okuyarak Connection String'i alıyoruz.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json") // Bağlantı dizgemiz burada
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // SQLite'ı kullanmasını söylüyoruz.
            builder.UseSqlite(connectionString);

            // DbContext'i Options ile oluşturup geri döndürüyoruz.
            return new ApplicationDbContext(builder.Options);
        }
    }
}