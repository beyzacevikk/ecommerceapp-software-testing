# 🛒 ECommerceApp — Yazilim Test ve Kalitesi Vize Projesi

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet" />
  <img src="https://img.shields.io/badge/Test%20Framework-NUnit%204.x-brightgreen" />
  <img src="https://img.shields.io/badge/Testler-25%20adet-blue" />
  <img src="https://img.shields.io/badge/PASS-17-success" />
  <img src="https://img.shields.io/badge/FAIL-8-red" />
  <img src="https://img.shields.io/badge/Bugs-8%20adet%20(kasitli)-orange" />
</p>

> **Piri Reis Universitesi | Yazilim Test ve Kalitesi Dersi — Vize Projesi**  
> Hazirlayan: **Beyza Cevik**  
> Tarih: 26.04.2026

---

## 📋 Icindekiler

- [Proje Hakkinda](#-proje-hakkinda)
- [Ogrenme Hedefleri](#-ogrenme-hedefleri)
- [Sistem Mimarisi](#-sistem-mimarisi)
- [Dosya Yapisi](#-dosya-yapisi)
- [Kasitli Yerlestirilen Buglar](#-kasitli-yerlestirilen-buglar)
- [Test Metodolojileri](#-test-metodolojileri)
- [Test Sonuclari](#-test-sonuclari)
- [Kurulum ve Calistirma](#-kurulum-ve-calistirma)
- [Detayli Rapor](#-detayli-rapor)

---

## 🎯 Proje Hakkinda

Bu proje, **bilerek hatali (buggy) yazilmis** bir e-ticaret uygulamasini ve bu hatalari yakalamak icin tasarlanmis kapsamli bir test suites'ini kapsamaktadir.

### Senaryo

Bir e-ticaret sisteminde kullanici asagidaki adimlari takip eder:

```
Kullanici → Urun Secer → Sepete Ekler → Siparis Verir → Odeme Yapar
```

Ancak sistemde **8 adet kasitli bug** bulunmaktadir. Bu buglari farkli test yontemleriyle tespit etmek projenin temel amacini olusturmaktadir.

---

## 🎓 Ogrenme Hedefleri

| Hedef | Aciklama |
|---|---|
| ✅ White Box Test | Kaynak koda bakarak ic mantigi, dal ve kosul kapsamini test etmek |
| ✅ Black Box Test | Ic kodu bilmeden yalnizca girdi/cikti davranisini dogrulamak |
| ✅ Gray Box Test | Kismi ic bilgiyle durum gecislerini ve sinir degerlerini test etmek |
| ✅ Integration Test | Birden fazla bilesenin birlikte dogru calistigini dogrulamak |
| ✅ Bug Raporlama | Hatalari tespit edip belgelemek ve duzeltme onerisi sunmak |

---

## 🏗 Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                        KULLANICI                            │
└─────────────────────┬───────────────────────────────────────┘
                      │
          ┌───────────▼───────────┐
          │      Program.cs       │  ← Uygulama giris noktasi
          └───────────┬───────────┘
                      │
     ┌────────────────┼────────────────┐
     │                │                │
┌────▼────┐    ┌──────▼──────┐  ┌─────▼──────┐
│ Urun    │    │   Sepet     │  │  Siparis   │
│ (Product│    │   (Cart.cs) │  │  Servisi   │
│  .cs)   │    │             │  │ (OrderSvc) │
│         │◄───┤  SepetOgesi │  │            │
│ -Fiyat  │    │  -UrunEkle  │  │ -Olustur   │
│ -Stok   │    │  -Toplam    │  │ -Odeme     │
└─────────┘    └─────────────┘  │ -Iptal     │
                                └────────────┘
```

### Siniflar ve Sorumluluklar

| Sinif | Dosya | Sorumluluk |
|---|---|---|
| `Urun` | `Core/Product.cs` | Urun bilgisi, fiyat guncelleme, stok yonetimi |
| `SepetOgesi` | `Core/Cart.cs` | Sepetteki tek bir kalemin urun + adet bilgisi |
| `Sepet` | `Core/Cart.cs` | Urun ekleme/cikarma, toplam ve indirim hesaplama |
| `Siparis` | `Core/OrderService.cs` | Siparis modeli, durum takibi |
| `SiparisServisi` | `Core/OrderService.cs` | Siparis olusturma, odeme isleme, iptal yonetimi |

---

## 📁 Dosya Yapisi

```
ECommerceApp/
│
├── Core/                              # Is mantigi katmani
│   ├── Product.cs                     # Urun sinifi — BUG #1, BUG #2
│   ├── Cart.cs                        # Sepet sinifi — BUG #3, BUG #4, BUG #5
│   └── OrderService.cs                # Siparis servisi — BUG #6, BUG #7, BUG #8
│
├── Tests/                             # Test projesi
│   ├── UnitTests/
│   │   ├── ProductCartTests.cs        # White Box Unit Testler (10 test)
│   │   └── BlackBoxGrayBoxTests.cs    # Black Box + Gray Box Testler (10 test)
│   └── IntegrationTests/
│       └── IntegrationTests.cs        # Entegrasyon Testleri (5 test)
│
├── Program.cs                         # Uygulama giris noktasi
├── ECommerceApp.csproj                # Ana proje dosyasi
├── ECommerceApp.Tests.csproj          # Test proje dosyasi
├── ECommerceApp.sln                   # Solution dosyasi
├── README.md                          # Bu dosya
└── REPORT.md                          # Detayli test raporu
```

---

## 🐛 Kasitli Yerlestirilen Buglar

Bu projede toplam **8 adet bug** bilerek yerlestirilmistir. Testlerin bu buglari yakalaması beklenmektedir.

### BUG #1 — Negatif Fiyat Kabulu
```csharp
// Core/Product.cs
public void FiyatGuncelle(decimal yeniFiyat)
{
    Fiyat = yeniFiyat; // ❌ Negatif deger kontrolu YOK
}
```
**Sorun:** `-500` gibi negatif bir fiyat set edilebiliyor.  
**Beklenen Davranis:** `ArgumentException` firlatilmali.  
**Yakalayan Test:** `TC-02` (FAIL)

---

### BUG #2 — Stok Negatife Dusebiliyor
```csharp
// Core/Product.cs
public void StokAzalt(int miktar)
{
    StokMiktari -= miktar; // ❌ Sinir kontrolu YOK
}
```
**Sorun:** Stokta 5 urun varken 15 azaltilirsa stok `-10` oluyor.  
**Beklenen Davranis:** Stok `0`'da kalmali ya da exception firlatilmali.  
**Yakalayan Test:** `TC-04` (FAIL)

---

### BUG #3 — Ayni Urun Duplicate Olarak Ekleniyor
```csharp
// Core/Cart.cs
public void UrunEkle(Urun urun, int adet)
{
    _ogeler.Add(new SepetOgesi(urun, adet)); // ❌ Mevcut urun kontrolu YOK
}
```
**Sorun:** Ayni urun iki kez eklenince sepette 2 ayri satir olusuyor, adet birlesmiyor.  
**Beklenen Davranis:** Mevcut urun varsa adedi arttirilmali.  
**Yakalayan Test:** `TC-09` (FAIL)

---

### BUG #4 — Stok Asimi Kontrolu Yok
```csharp
// Core/Cart.cs
public void UrunEkle(Urun urun, int adet)
{
    // ❌ adet > urun.StokMiktari kontrolu YOK
    _ogeler.Add(new SepetOgesi(urun, adet));
}
```
**Sorun:** Stokta 3 adet varken 99 adet sepete eklenebiliyor.  
**Beklenen Davranis:** `InvalidOperationException` firlatilmali.  
**Yakalayan Test:** `TC-GB-04` (FAIL)

---

### BUG #5 — %100'den Fazla Indirim Negatif Toplam Yapiyor
```csharp
// Core/Cart.cs
public decimal IndirimliFiyatHesapla(decimal indirimYuzdesi)
{
    decimal toplam = ToplamHesapla();
    return toplam - (toplam * indirimYuzdesi / 100); // ❌ Yuzde aralik kontrolu YOK
}
```
**Sorun:** `%150` indirim verilince toplam `-7500 TL` cikiyor.  
**Beklenen Davranis:** Yuzde `[0, 100]` arasinda olmali, aksi halde exception.  
**Yakalayan Test:** `TC-08` (FAIL)

---

### BUG #6 — Bos Sepete Siparis Verilebiliyor
```csharp
// Core/OrderService.cs
public Siparis SiparisOlustur(Sepet sepet, string musteriAdi)
{
    // ❌ Sepet bos kontrolu YOK
    decimal toplam = sepet.ToplamHesapla();
    ...
}
```
**Sorun:** Hic urun eklenmeden `0 TL`'lik siparis olusturuluyor.  
**Beklenen Davranis:** `InvalidOperationException` firlatilmali.  
**Yakalayan Test:** `TC-BB-02` (FAIL)

---

### BUG #7 — Eksik Odeme Onaylaniyor
```csharp
// Core/OrderService.cs
public bool OdemeYap(Siparis siparis, OdemeTuru odemeTuru, decimal odenenTutar)
{
    // ❌ odenenTutar < siparis.ToplamTutar kontrolu YOK
    siparis.Durum = SiparisDurumu.Onaylandi;
    return true; // Her zaman true donuyor!
}
```
**Sorun:** `1000 TL`'lik siparise `1 TL` odense bile sistem `true` dondurüyor.  
**Beklenen Davranis:** Eksik odeme icin `false` donmeli.  
**Yakalayan Test:** `TC-BB-03` (FAIL)

---

### BUG #8 — Teslim Edilmis Siparis Iptal Edilebiliyor
```csharp
// Core/OrderService.cs
public bool SiparisIptalEt(int siparisId)
{
    var siparis = _siparisler.Find(s => s.SiparisId == siparisId);
    // ❌ siparis.Durum == TeslimEdildi kontrolu YOK
    siparis.Durum = SiparisDurumu.IptalEdildi;
    return true;
}
```
**Sorun:** Musteriye teslim edilmis siparis de iptal edilebiliyor.  
**Beklenen Davranis:** `TeslimEdildi` durumundaki siparis iptal edilememeli, `false` donmeli.  
**Yakalayan Test:** `TC-BB-04` (FAIL)

---

## 🧪 Test Metodolojileri

### 1. White Box Test (Birim Testi — Ic Kod Testi)

> Kaynak koda tam erisim ile ic dal, kosul ve dongu kapsamlari test edilir.

**Dosya:** `Tests/UnitTests/ProductCartTests.cs`

| Test ID | Test Adi | Sonuc |
|---|---|---|
| TC-01 | FiyatGuncelle_GecerliFiyat_BasariylaGuncellenir | ✅ PASS |
| TC-02 | FiyatGuncelle_NegatifFiyat_HataFirlatmali | ❌ FAIL |
| TC-03 | StokAzalt_GecerliMiktar_DogruAzaltir | ✅ PASS |
| TC-04 | StokAzalt_StokuAsanMiktar_SifirdaKalmali | ❌ FAIL |
| TC-05 | StokVarMi_StokSifir_YanlisDonerMi | ✅ PASS |
| TC-06 | UrunEkle_GecerliUrun_ToplamDogruHesaplanir | ✅ PASS |
| TC-07 | UrunEkle_SifirAdet_ArgumentExceptionFirlatmali | ✅ PASS |
| TC-08 | IndirimliFiyatHesapla_YuzdeyuzdenFazlaIndirim_NegatifOlmamali | ❌ FAIL |
| TC-09 | UrunEkle_AyniUrunIkiKez_AdediBirlesmeli | ❌ FAIL |
| TC-10 | UrunCikar_MevcutUrun_Sepetten_Cikar | ✅ PASS |

---

### 2. Black Box Test (Davranis Testi)

> Ic koda bakilmadan yalnizca sistem girislerine karsilik gelen ciktilar dogrulanir.

**Dosya:** `Tests/UnitTests/BlackBoxGrayBoxTests.cs` — `KaraSutunTestleri` sinifi

| Test ID | Test Adi | Sonuc |
|---|---|---|
| TC-BB-01 | SiparisOlustur_GecerliSepet_DogruToplamlaGeri_Doner | ✅ PASS |
| TC-BB-02 | SiparisOlustur_BosSepet_HataFirlatmali | ❌ FAIL |
| TC-BB-03 | OdemeYap_YetersizTutar_YanlisDonmeli | ❌ FAIL |
| TC-BB-04 | SiparisIptalEt_TeslimEdilmisSiparis_IptalEdilmemeli | ❌ FAIL |
| TC-BB-05 | SiparisIptalEt_OlmayanSiparis_YanlisDonerMi | ✅ PASS |

---

### 3. Gray Box Test (Durum Gecis Testi)

> Kismi ic bilgiyle durum makinesi ve sinir degerleri test edilir.

**Dosya:** `Tests/UnitTests/BlackBoxGrayBoxTests.cs` — `GriSutunTestleri` sinifi

| Test ID | Test Adi | Sonuc |
|---|---|---|
| TC-GB-01 | SiparisDurumu_OdemeSonrasi_OnayanlandiOlmali | ✅ PASS |
| TC-GB-02 | SiparisIptalEt_BeklemedekiSiparis_IptalEdildiOlmali | ✅ PASS |
| TC-GB-03 | UrunEkle_TamStokKadar_BasariliOlmali | ✅ PASS |
| TC-GB-04 | UrunEkle_StoktenFazla_HataFirlatmali | ❌ FAIL |
| TC-GB-05 | SiparisOlustur_SonraSepet_TemizlenmisOlmali | ✅ PASS |

---

### 4. Integration Test (Entegrasyon Testi)

> `Urun`, `Sepet` ve `SiparisServisi` bilesenleri birlikte, uctan uca test edilir.

**Dosya:** `Tests/IntegrationTests/IntegrationTests.cs`

| Test ID | Test Adi | Sonuc |
|---|---|---|
| TC-INT-01 | TamAkis_GecerliSenaryo_BasariylaTamamlanmali | ✅ PASS |
| TC-INT-02 | SiparisOlustur_SonraSepetBosOlmali | ✅ PASS |
| TC-INT-03 | CokluSiparis_HerSipariseBenzersizIdVerilmeli | ✅ PASS |
| TC-INT-04 | IndirimliFiyat_DogruUygulanmali | ✅ PASS |
| TC-INT-05 | TumSiparisleriGetir_TumSiparislerListelenmeli | ✅ PASS |

---

## 📊 Test Sonuclari

```
╔══════════════════════╦═════════╦══════╦══════╗
║ Test Turu            ║ Toplam  ║ PASS ║ FAIL ║
╠══════════════════════╬═════════╬══════╬══════╣
║ Unit (White Box)     ║   10    ║   6  ║   4  ║
║ Black Box            ║    5    ║   2  ║   3  ║
║ Gray Box             ║    5    ║   4  ║   1  ║
║ Integration          ║    5    ║   5  ║   0  ║
╠══════════════════════╬═════════╬══════╬══════╣
║ TOPLAM               ║   25    ║  17  ║   8  ║
╚══════════════════════╩═════════╩══════╩══════╝
```

> 8 FAIL testin tamami kasitli yerlestirilmis bug'lara karsilik gelmektedir.  
> Detayli analiz icin [REPORT.md](REPORT.md) dosyasina bakiniz.

---

## ⚙️ Kurulum ve Calistirma

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- NUnit 4.x *(NuGet uzerinden otomatik yuklenir)*

### 1. Repoyu Klonla

```bash
git clone https://github.com/KULLANICI_ADIN/ecommerceapp-software-testing.git
cd ecommerceapp-software-testing
```

### 2. Bagimliliklari Yukle

```bash
cd ECommerceApp
dotnet restore
```

### 3. Ana Uygulamayi Calistir

```bash
dotnet run --project ECommerceApp.csproj
```

### 4. Testleri Calistir

```bash
cd Tests
dotnet restore
dotnet test
```

**Beklenen Cikti:**
```
Toplam: 25 test
Basarili: 17
Basarisiz: 8  ← Kasitli buglar
```

---

## 📄 Detayli Rapor

Her FAIL testin **nedeni**, **ilgili bug** ve **duzeltme onerisi** icin:

👉 [REPORT.md](REPORT.md)

---

## 🏫 Proje Bilgileri

| Alan | Bilgi |
|---|---|
| Universite | Piri Reis Universitesi |
| Ders | Yazilim Test ve Kalitesi |
| Donem | 2025–2026 Bahar |
| Ogrenci | Beyza Cevik |
| Ogretim Uyesi | Emrah Saricibek |
| Teslim Tarihi | 29.04.2026 |
