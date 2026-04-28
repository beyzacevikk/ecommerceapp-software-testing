# ECommerceApp - Yazilim Test ve Kalitesi Vize Projesi

Piri Reis Universitesi | Yazilim Test ve Kalitesi Dersi  
Hazirlayan: Beyza Cevik

---

## Proje Hakkinda

C# ve NUnit kullanilarak gelistirilmis, bilerek hatali yazilmis bir e-ticaret sistemi ve test suite'i.  
Kullanici urun secer, sepete ekler, siparis verir ve odeme yapar.  
Sistemdeki bug'lar testlerle yakalanir.

---

## Dosya Yapisi

```
ECommerceApp/
├── Core/
│   ├── Product.cs          # Urun modeli (BUG #1, #2)
│   ├── Cart.cs             # Sepet islemleri (BUG #3, #5)
│   └── OrderService.cs     # Siparis & odeme (BUG #6, #7, #8)
├── Tests/
│   ├── UnitTests/
│   │   ├── ProductCartTests.cs        # White Box Unit Testler
│   │   └── BlackBoxGrayBoxTests.cs    # Black Box & Gray Box Testler
│   └── IntegrationTests/
│       └── IntegrationTests.cs        # Integration Testler
├── Program.cs
├── ECommerceApp.csproj
├── ECommerceApp.sln
└── REPORT.md
```

---

## Kurulum ve Calistirma

### Gereksinimler
- .NET 8 SDK
- NUnit 4.x (NuGet uzerinden otomatik yuklenir)

### Ana Uygulamayi Calistir
```bash
dotnet run --project ECommerceApp.csproj
```

### Testleri Calistir
```bash
cd Tests
dotnet restore
dotnet test
```

---

## Test Kategorileri

| Tur | Dosya | Amac |
|---|---|---|
| **White Box** | `UnitTests/ProductCartTests.cs` | Ic kod dallarini test eder |
| **Black Box** | `UnitTests/BlackBoxGrayBoxTests.cs` | Girdi/cikti davranisini test eder |
| **Gray Box** | `UnitTests/BlackBoxGrayBoxTests.cs` | Durum gecislerini test eder |
| **Integration** | `IntegrationTests/IntegrationTests.cs` | Bilesenler arasi entegrasyonu test eder |

---

## Test Sonuclari

```
Toplam: 25 test
Basarili: 17
Basarisiz: 8  <-- Bilerek yerlestirilmis bug'lar
```

---

## Bug Ozeti

| Bug # | Konum | Aciklama | Test |
|---|---|---|---|
| BUG-1 | `Urun.FiyatGuncelle()` | Negatif fiyata izin veriliyor | TC-02 FAIL |
| BUG-2 | `Urun.StokAzalt()` | Stok negatife dusuyor | TC-04 FAIL |
| BUG-3 | `Sepet.UrunEkle()` | Duplicate urun birlesmesi yok | TC-09 FAIL |
| BUG-5 | `Sepet.IndirimliFiyatHesapla()` | %100+ indirimde negatif toplam | TC-08 FAIL |
| BUG-6 | `SiparisServisi.SiparisOlustur()` | Bos sepete siparis verilebiliyor | TC-BB-02 FAIL |
| BUG-7 | `SiparisServisi.OdemeYap()` | Eksik odeme onaylaniyor | TC-BB-03 FAIL |
| BUG-8 | `SiparisServisi.SiparisIptalEt()` | Teslim siparisi iptal edilebiliyor | TC-BB-04 FAIL |

Detayli analiz: [REPORT.md](REPORT.md)
