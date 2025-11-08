using Microsoft.AspNetCore.Identity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Documents
{
    public class ProposalDocument : IDocument
    {
        public Proposal Model { get; }
        public ApplicationUser User { get; }

        public ProposalDocument(Proposal model, ApplicationUser user)
        {
            Model = model;
            User = user;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginHorizontal(40);
                    page.MarginVertical(30);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        // --- HATA DÜZELTMESİ BURADA ---
        // 'container' (Header) sadece tek bir çocuk alabilir.
        // Bu yüzden 'Row' ve 'BorderBottom'u tek bir 'Column' içine sardık.
        void ComposeHeader(IContainer container)
        {
            container.Column(col => // <-- YENİ EKLENEN ANA SÜTUN
            {
                // Sütunun 1. elemanı (Satır)
                col.Item().Row(row =>
                {
                    // Sol Taraf: Teklifi Veren (Sizin Şirket Bilgileri)
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(User.CompanyName ?? "Firma Adı")
                            .Bold().FontSize(20).FontColor(Colors.Blue.Darken2);

                        col.Item().Text(User.CompanyAddress ?? "Adres Bilgisi Yok");
                        col.Item().Text(User.Email);
                        col.Item().Text(User.PhoneNumber ?? "");
                    });

                    // Sağ Taraf: Teklif Başlığı
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text("Doğrudan Alım Fiyat Teklifi")
                            .Bold().FontSize(22).FontColor(Colors.Grey.Darken1);

                        col.Item().AlignRight().Text(Model.ProposalNumber ?? $"#{Model.Id}")
                            .SemiBold().FontSize(14);
                        col.Item().AlignRight().Text($"Tarih: {Model.IssueDate:d}");
                        col.Item().AlignRight().Text($"Geçerlilik: {Model.ExpiryDate:d}");
                    });
                });

                // Sütunun 2. elemanı (Alt Çizgi)
                col.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            });
        }
        // --- HATA DÜZELTMESİ BİTTİ ---

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                // Müşteri Bilgileri
                col.Item().Element(ComposeCustomerInfo);

                // Mektup Metni
                col.Item().PaddingTop(25).Column(introCol =>
                {
                    introCol.Item().Text($"Sayın {Model.CustomerName},")
                        .SemiBold().FontSize(14);

                    introCol.Item().PaddingTop(10).Text(
                        $"{User.CompanyName ?? "Firmamız"} tarafından {Model.CustomerName} kurumuna/şahsına doğrudan temin fiyat teklifi mektubudur. " +
                        "Aşağıda adı geçen ürünler/hizmetler için KDV dahil fiyatlarımız düzenlenmiştir."
                    ).FontSize(10);

                    // Şartlı İskonto Metni
                    if (Model.DiscountValue > 0)
                    {
                        string discountText = "";
                        if (Model.DiscountType == DiscountType.Percentage)
                        {
                            discountText = $"% {Model.DiscountValue}";
                        }
                        else
                        {
                            discountText = $"{Model.TotalDiscountAmount:C2}";
                        }

                        introCol.Item().PaddingTop(5).Text(
                            $"(Ara toplam üzerinden {discountText} iskonto uygulanmıştır.)"
                        ).Italic().FontSize(10);
                    }
                });

                // Ürün/Hizmet Tablosu
                col.Item().PaddingTop(25).Element(ComposeTable);

                // Toplamlar Bölümü
                col.Item().AlignRight().Element(ComposeTotals);

                // Notlar Bölümü
                if (!string.IsNullOrWhiteSpace(Model.Notes))
                    col.Item().PaddingTop(25).Element(ComposeNotes);

                // İmza Bölümü
                col.Item().PaddingTop(40).Element(ComposeSignature);
            });
        }

        // MÜŞTERİ BİLGİLERİ (KİME)
        void ComposeCustomerInfo(IContainer container)
        {
            container.ShowEntire().Column(col =>
            {
                col.Item().Text("TEKLİF ALAN (MÜŞTERİ)")
                    .SemiBold().FontSize(10).FontColor(Colors.Grey.Darken1);

                col.Item().Text(Model.CustomerName).Bold();
                col.Item().Text(Model.CustomerAddress);
                col.Item().Text(Model.CustomerEmail);
            });
        }

        // ÜRÜN TABLOSU
        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(5); // Açıklama
                    columns.ConstantColumn(50); // Miktar
                    columns.ConstantColumn(80); // Birim Fiyat
                    columns.ConstantColumn(50); // KDV
                    columns.ConstantColumn(90); // Ara Toplam
                });

                // Başlık
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Açıklama");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Miktar");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Birim Fiyat");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("KDV");
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Satır Toplamı");
                });

                // Satırlar
                foreach (var item in Model.Items)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                        .Text(item.Name);

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                        .AlignCenter().Text(item.Quantity);

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                        .AlignRight().Text($"{item.UnitPrice:C}");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                        .AlignCenter().Text($"%{item.VATRate}");

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5)
                        .AlignRight().Text($"{item.CalculatedSubtotal:C}");
                }
            });
        }

        // GENEL TOPLAMLAR BÖLÜMÜ
        void ComposeTotals(IContainer container)
        {
            var culture = new CultureInfo("tr-TR");

            container.Width(280).Column(col =>
            {
                // Ara Toplam
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("Ara Toplam");
                    row.ConstantItem(100).AlignRight().Text($"{Model.TotalSubtotal:C}");
                });

                // İskonto (Sadece varsa göster)
                if (Model.TotalDiscountAmount > 0)
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("İskonto").FontColor(Colors.Red.Medium);
                        row.ConstantItem(100).AlignRight().Text($"-{Model.TotalDiscountAmount:C}").FontColor(Colors.Red.Medium);
                    });

                    col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Row(row =>
                    {
                        row.RelativeItem().Text("Net Toplam").Bold();
                        row.ConstantItem(100).AlignRight().Text($"{Model.TotalNetTotal:C}").Bold();
                    });
                }

                // KDV
                col.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("Toplam KDV");
                    row.ConstantItem(100).AlignRight().Text($"{Model.TotalVATAmount:C}");
                });

                // Genel Toplam
                col.Item().PaddingTop(5).BorderTop(2).BorderColor(Colors.Black).Row(row =>
                {
                    row.RelativeItem().Text("Genel Toplam").Bold().FontSize(14);
                    row.ConstantItem(100).AlignRight().Text($"{Model.TotalGrandTotal:C}").Bold().FontSize(14);
                });
            });
        }

        // NOTLAR BÖLÜMÜ
        void ComposeNotes(IContainer container)
        {
            container.Background(Colors.Grey.Lighten4).Padding(10).Column(col =>
            {
                col.Item().Text("Ek Notlar ve Şartlar").SemiBold();
                col.Item().Text(Model.Notes);
            });
        }

        // İMZA BÖLÜMÜ
        void ComposeSignature(IContainer container)
        {
            container.Row(row =>
            {
                // Banka Bilgileri (Varsa)
                if (!string.IsNullOrWhiteSpace(User.CompanyIBAN))
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Banka Bilgileri").SemiBold();
                        col.Item().Text(User.CompanyName);
                        col.Item().Text($"IBAN: {User.CompanyIBAN}");
                    });
                }
                else
                {
                    row.RelativeItem(); // Boşluk
                }

                // İmza Alanı (Yetkili)
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("Saygılarımızla,");
                    col.Item().PaddingTop(15).AlignCenter().Text(User.CompanyName ?? "Firma Kaşesi");
                    col.Item().PaddingTop(25).AlignCenter().Text("_________________________");
                    col.Item().AlignCenter().Text(User.ContactFullName ?? "Yetkili Adı Soyadı");
                    col.Item().AlignCenter().Text(User.ContactTitle ?? "Yetkili Unvanı");
                });
            });
        }

        // FOOTER (Alt Bilgi)
        void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Row(row =>
            {
                row.RelativeItem().Text(User.CompanyName ?? "Firma");
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }
    }
}