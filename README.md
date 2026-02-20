# Teklif Sepeti 📄🛒

**Teklif Sepeti**, serbest çalışanların ve küçük işletmelerin müşterilerine hızlı ve profesyonel görünümlü fiyat teklifleri oluşturmasını kolaylaştıran web tabanlı bir uygulamadır. Word veya Excel ile uğraşmak yerine, teklif kalemlerinizi girip anında hesaplanmış, şık bir PDF çıktısı alabilirsiniz. Proje nihayete erdirilmemiştir. Geliştirilmeye devam edecektir.

## 🎯 Amaç ve Hedef Kitle

Bu proje, özellikle aşağıdaki kullanıcı gruplarının hayatını kolaylaştırmayı hedefler:
* Serbest Çalışanlar (Freelancerlar)
* Küçük Atölyeler ve İşletmeler
* Hızlı ve standart tekliflere ihtiyaç duyan satış ekipleri.

## ✨ Temel Özellikler (MVP - Minimum Uygulanabilir Ürün)

Projenin bu ilk versiyonu aşağıdaki temel işlevleri sunmaktadır:

* **Teklif Oluşturma:**
    * Müşteri bilgilerini (Ad/Unvan, E-posta, Adres) her teklif için manuel olarak girme.
    * Dinamik olarak teklif kalemleri (ürün/hizmet) ekleme ve silme.
    * Her kalem için Açıklama, Miktar, Birim Fiyat ve KDV Oranını manuel girme.
    * Arayüzde anlık (real-time) ara toplam, KDV ve genel toplam hesaplaması (JavaScript ile).
* **Veri Kaydı:**
    * Oluşturulan tekliflerin tüm detayları ve sunucu tarafında (C#) yeniden hesaplanmış güvenli toplamlarla SQLite veritabanına kaydedilmesi.
    * Otomatik Teklif Numarası ve Teklif Tarihi ataması.
* **Listeleme ve Görüntüleme:**
    * Kaydedilen tüm tekliflerin ana sayfada listelenmesi (`Index`).
    * Bir teklife tıklandığında tüm detaylarının ayrı bir sayfada gösterilmesi (`ProposalDetails`).
* **PDF Üretimi:**
    * Teklif detay sayfasından, seçili teklifin profesyonel bir PDF dosyası olarak indirilmesi (QuestPDF kullanılarak).

## 💻 Teknoloji Yığını

* **Backend:** C#, ASP.NET Core Razor Pages (.NET 8)
* **Veritabanı:** SQLite, Entity Framework Core
* **Frontend:** HTML, CSS, Bootstrap 5, JavaScript
* **PDF Üretimi:** QuestPDF

## 🚧 Mevcut Durum

Proje şu anda yukarıda listelenen MVP özelliklerini içeren, çalışır durumdadır. Kod tabanı GitHub'a yüklenmiştir.

## 🚀 Gelecek Planları (Yeni Dallar ile Eklenecekler)

* Kullanıcı Profili ve Şirket Bilgileri Yönetimi (PDF'e otomatik ekleme için). ✅
* Gelişmiş İskonto Uygulama Mantığı.✅
* Teklifleri Silme Fonksiyonu.✅
* Müşteri Adresi için İl/İlçe seçimi gibi arayüz iyileştirmeleri.

## 🛠️ Kurulum ve Çalıştırma (Geliştirme Ortamı)

1.  [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)'nın kurulu olduğundan emin olun.
2.  Bu depoyu klonlayın: `git clone https://github.com/BalciAbdulkadir/Teklif_Sepeti.git`
3.  Proje klasörüne gidin: `cd Teklif_Sepeti`
4.  Veritabanını oluşturun/güncelleyin: `dotnet ef database update`
5.  Uygulamayı çalıştırın: `dotnet run`
6.  Tarayıcınızda açılan adrese gidin (genellikle `https://localhost:XXXX`).

## 📜 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.
