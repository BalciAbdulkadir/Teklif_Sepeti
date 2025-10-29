using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Teklif_Sepeti.Models; // Kendi Model namespace'in

namespace Teklif_Sepeti.Documents
{
    public class ProposalDocument : IDocument
    {
        // Bu property, PDF'i oluştururken kullanacağımız veriyi (teklifi) tutacak.
        public Proposal Proposal { get; }

        // Constructor: Bu sınıfı oluştururken hangi teklifi kullanacağımızı belirtiyoruz.
        public ProposalDocument(Proposal proposal)
        {
            Proposal = proposal;
        }

        // QuestPDF'in ana metodu: Belge meta verilerini ayarlar.
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // QuestPDF'in asıl işi yapan metodu: PDF'in içeriğini oluşturur.
        public void Compose(IDocumentContainer container)
        {
            // Türkçe kültürünü ayarlıyoruz ki para birimi vb. doğru görünsün
            CultureInfo culture = new CultureInfo("tr-TR");

            container
                .Page(page =>
                {
                    // Sayfa ayarları: A4, kenar boşlukları vb.
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(style => style.FontSize(10).FontFamily(Fonts.Arial)); // Varsayılan yazı tipi

                    // Sayfa Başlığı (Antet)
                    page.Header().Element(ComposeHeader);

                    // Ana İçerik
                    page.Content().Element(ComposeContent);

                    // Sayfa Altlığı (Footer)
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber(); // Sayfa numarasını ekle
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
        }

        // Başlık (Antet) kısmını oluşturan metot
        void ComposeHeader(IContainer container)
        {
            container.Column(column => // Ana container'ı bir Column yapalım ki altına eleman ekleyebilelim
            {
                // 1. ELEMAN: İçerik satırı (Şirket Adı, Teklif No vs.)
                column.Item().Row(row =>
                {
                    // Sol taraf (Şirket Logosu/Adı) /DÜZENLENECEK
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("SİZİN ŞİRKETİNİZ A.Ş.").SemiBold().FontSize(14);
                        col.Item().Text("Adresiniz, İletişim Bilgileriniz");
                    });

                    // Sağ taraf (Teklif Numarası ve Tarihler)
                    row.ConstantItem(150).Column(col =>
                    {
                        col.Item().Text($"Teklif No: {Proposal.ProposalNumber}").Bold();
                        col.Item().Text($"Tarih: {Proposal.IssueDate:dd.MM.yyyy}");
                        col.Item().Text($"Geçerlilik: {Proposal.ExpiryDate:dd.MM.yyyy}");
                    });
                });

                // 2. ELEMAN: Boşluk ve Alt Çizgi
                // Önce boşluk verip sonra çizgiyi çekiyoruz.
                column.Item().PaddingTop(0.5f, Unit.Centimetre).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
            });
        }

        // Ana İçeriği oluşturan metot
        void ComposeContent(IContainer container)
        {
            CultureInfo culture = new CultureInfo("tr-TR"); // Türkçe formatı için

            container.PaddingTop(0.5f, Unit.Centimetre).Column(column =>
            {
                // Müşteri Bilgileri
                column.Item().Row(row =>
                {
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(col =>
                    {
                        col.Item().Text("Müşteri Bilgileri").SemiBold().FontSize(12);
                        col.Item().PaddingTop(5).Text(Proposal.CustomerName).Bold();
                        col.Item().Text(Proposal.CustomerAddress);
                        col.Item().Text(Proposal.CustomerEmail);
                    });
                    // İleride buraya başka bir kutu (örn: Proje Adı) eklenebilir.
                    row.ConstantItem(50); // Boşluk
                    row.RelativeItem(); // Şimdilik boş sağ kutu
                });

                // Teklif Kalemleri Tablosu
                column.Item().PaddingTop(1, Unit.Centimetre).Element(ComposeTable);

                // Toplamlar ve Notlar
                column.Item().PaddingTop(0.5f, Unit.Centimetre).Row(row =>
                {
                    // Sol Taraf: Notlar
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Notlar").SemiBold().FontSize(12);
                        col.Item().PaddingTop(5).Text(Proposal.Notes ?? "Ek not bulunmamaktadır.");
                    });

                    // Sağ Taraf: Toplamlar
                    row.ConstantItem(150).Column(col =>
                    {
                        col.Item().AlignRight().Text("Ara Toplam: " + Proposal.TotalSubtotal.ToString("C", culture));
                        col.Item().AlignRight().Text("Toplam KDV: " + Proposal.TotalVATAmount.ToString("C", culture));
                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(5).AlignRight().Text(text =>
                        {
                            text.Span("GENEL TOPLAM: ").SemiBold(); // İlk kısım
                            text.Span(Proposal.TotalGrandTotal.ToString("C", culture)).Bold().FontSize(12); // İkinci kısım (farklı stil)
                        });
                    });
                });

                // İmza Alanı (Opsiyonel)
                column.Item().PaddingTop(2, Unit.Centimetre).Row(row =>
                {
                    row.RelativeItem().Column(col => {
                        col.Item().Text("Teklifi Hazırlayan");
                        // TODO: Kullanıcı profilinden Ad/Soyad buraya gelecek
                        col.Item().PaddingTop(1, Unit.Centimetre).Text(".........................");
                        col.Item().Text("[Adınız Soyadınız]");
                        col.Item().Text("[Unvanınız]");
                    });
                    row.RelativeItem(); // Boşluk
                    row.RelativeItem(); // Boşluk
                });

            });
        }

        // Kalemler tablosunu oluşturan metot
        void ComposeTable(IContainer container)
        {
            CultureInfo culture = new CultureInfo("tr-TR"); // Türkçe formatı için

            container.Table(table =>
            {
                // Tablo sütunlarını tanımla
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5); // Açıklama için daha geniş
                    columns.RelativeColumn(1); // Miktar
                    columns.RelativeColumn(2); // Birim Fiyat
                    columns.RelativeColumn(2); // Satır Toplamı
                });

                // Tablo başlığını (header) çiz
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Açıklama").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text("Miktar").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Birim Fiyat").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Toplam").SemiBold();
                });

                // Her bir teklif kalemi için satırları çiz
                foreach (var item in Proposal.Items)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Name);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(item.UnitPrice.ToString("C", culture));
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(item.CalculatedSubtotal.ToString("C", culture));
                }
            });
        }
    }
}