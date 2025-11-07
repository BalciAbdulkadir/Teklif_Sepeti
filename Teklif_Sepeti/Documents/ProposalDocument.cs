using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Teklif_Sepeti.Models;

namespace Teklif_Sepeti.Documents
{
public class ProposalDocument : IDocument
    {
     // Teklif ve kullanıcı verilerini tutacak property'ler
   public Proposal Proposal { get; }
 public ApplicationUser User { get; }

    public ProposalDocument(Proposal proposal, ApplicationUser user)
        {
          Proposal = proposal;
 User = user;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // PDF'in yapısını oluşturuyorum
        public void Compose(IDocumentContainer container)
        {
            // Para birimini Türk Lirası yapıyorum
        var culture = new CultureInfo("tr-TR");

          container.Page(page =>
     {
     // A4 kağıt ayarları
             page.Size(PageSizes.A4);
         page.Margin(1.5f, Unit.Centimetre);
         page.DefaultTextStyle(style => style.FontSize(10).FontFamily(Fonts.Arial));

 // Antet, içerik ve altbilgi
    page.Header().Element(ComposeHeader);
        page.Content().Element(ComposeContent);

   // Sayfa numarası
     page.Footer().AlignCenter().Text(text =>
                {
         text.CurrentPageNumber();
        text.Span(" / ");
  text.TotalPages();
});
            });
        }

        // Antet kısmını oluşturuyorum
        void ComposeHeader(IContainer container)
  {
         container.Column(column =>
            {
                column.Item().Row(row =>
         {
        // Şirket bilgileri
   row.RelativeItem().Column(col =>
        {
   col.Item().Text(User.CompanyName ?? "Şirket Adı Girilmemiş").SemiBold().FontSize(14);
          col.Item().Text(User.CompanyAddress ?? "Adres Girilmemiş");
  col.Item().Text($"Vergi Dairesi: {User.CompanyTaxOffice} - VKN: {User.CompanyTaxNumber}");
       col.Item().Text($"IBAN: {User.CompanyIBAN}");
   });

             // Teklif detayları
      row.ConstantItem(150).Column(col =>
       {
         col.Item().Text($"Teklif No: {Proposal.ProposalNumber}").Bold();
      col.Item().Text($"Tarih: {Proposal.IssueDate:dd.MM.yyyy}");
      col.Item().Text($"Geçerlilik: {Proposal.ExpiryDate:dd.MM.yyyy}");
     });
     });

           // Ayırıcı çizgi
  column.Item().PaddingTop(0.5f, Unit.Centimetre).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
         });
        }

  // Ana içeriği oluşturuyorum
     void ComposeContent(IContainer container)
        {
      var culture = new CultureInfo("tr-TR");

    container.PaddingTop(0.5f, Unit.Centimetre).Column(column =>
            {
             // Müşteri bilgilerini yerleştiriyorum
      column.Item().Row(row =>
          {
         row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(col =>
     {
 col.Item().Text("Müşteri Bilgileri").SemiBold().FontSize(12);
          col.Item().PaddingTop(5).Text(Proposal.CustomerName).Bold();
  col.Item().Text(Proposal.CustomerAddress);
      col.Item().Text(Proposal.CustomerEmail);
 });
      row.ConstantItem(50);
row.RelativeItem();
    });

   // Teklif kalemlerini tabloya ekliyorum
                column.Item().PaddingTop(1, Unit.Centimetre).Element(ComposeTable);

      // Not ve toplam tutarları gösteriyorum
   column.Item().PaddingTop(0.5f, Unit.Centimetre).Row(row =>
    {
           row.RelativeItem().Column(col =>
   {
 col.Item().Text("Notlar").SemiBold().FontSize(12);
          col.Item().PaddingTop(5).Text(Proposal.Notes ?? "Ek not bulunmamaktadır.");
        });

              // Toplam tutarlar
row.ConstantItem(150).Column(col =>
    {
               col.Item().AlignRight().Text("Ara Toplam: " + Proposal.TotalSubtotal.ToString("C", culture));
     col.Item().AlignRight().Text("Toplam KDV: " + Proposal.TotalVATAmount.ToString("C", culture));
  col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(5).AlignRight().Text(text =>
      {
       text.Span("GENEL TOPLAM: ").SemiBold();
         text.Span(Proposal.TotalGrandTotal.ToString("C", culture)).Bold().FontSize(12);
     });
   });
     });

            // İmza alanı
             column.Item().PaddingTop(2, Unit.Centimetre).Row(row =>
          {
             row.RelativeItem().Column(col => {
         col.Item().Text("Teklifi Hazırlayan");
  col.Item().Text(User.ContactFullName ?? "[Ad Soyad Girilmemiş]").Bold();
              col.Item().Text(User.ContactTitle ?? "[Unvan Girilmemiş]");
   });
       row.RelativeItem();
           row.RelativeItem();
            });
      });
        }

        // Teklif kalemlerinin tablosunu oluşturuyorum
        void ComposeTable(IContainer container)
        {
  var culture = new CultureInfo("tr-TR");

            container.Table(table =>
            {
       // Sütun genişliklerini ayarlıyorum
         table.ColumnsDefinition(columns =>
   {
            columns.RelativeColumn(5);  // Açıklama
   columns.RelativeColumn(1);  // Miktar
         columns.RelativeColumn(2);  // Fiyat
                  columns.RelativeColumn(2);  // Toplam
       });

                // Tablo başlıklarını oluşturuyorum
       table.Header(header =>
                {
  header.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Açıklama").SemiBold();
          header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text("Miktar").SemiBold();
             header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Birim Fiyat").SemiBold();
          header.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignRight().Text("Toplam").SemiBold();
        });

   // Teklif kalemlerini tabloya ekliyorum
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