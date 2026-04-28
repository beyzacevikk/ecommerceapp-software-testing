# E-Ticaret Uygulamasi - Test Raporu

**Proje:** ECommerceApp  
**Tarih:** 2026-04-29  
**Test Framework:** NUnit 4.x / .NET 8  
**Hazirlayan:** Beyza Cevik

---

## 1. Proje Genel Bakis

Bu projede bilerek hatali (bug) yazilmis bir e-ticaret sistemi gelistirilmis ve testlerle bu hatalar yakalanmistir. Sistem uc temel modulden olusmustur:

| Modul | Dosya | Aciklama |
|---|---|---|
| Urun | `Core/Product.cs` | Urun fiyat ve stok yonetimi |
| Sepet | `Core/Cart.cs` | Sepete ekleme, toplam hesaplama |
| Siparis | `Core/OrderService.cs` | Siparis olusturma, odeme, iptal |

---

## 2. Bilerek Yerlestirilen Bug'lar

| Bug # | Konum | Aciklama |
|---|---|---|
| BUG-1 | `Urun.FiyatGuncelle()` | Negatif fiyat kabul ediliyor |
| BUG-2 | `Urun.StokAzalt()` | Stok negatife dusebiliyor |
| BUG-3 | `Sepet.UrunEkle()` | Ayni urun tekrar eklenince duplicate olusturuluyor |
| BUG-4 | `Sepet.UrunEkle()` | Stok kontrolu yapilmiyor, stoktan fazla urun eklenebiliyor |
| BUG-5 | `Sepet.IndirimliFiyatHesapla()` | %100'den fazla indirimde toplam negatif oluyor |
| BUG-6 | `SiparisServisi.SiparisOlustur()` | Bos sepete siparis verilebiliyor |
| BUG-7 | `SiparisServisi.OdemeYap()` | Eksik odeme miktari yine "basarili" sayiliyor |
| BUG-8 | `SiparisServisi.SiparisIptalEt()` | Teslim edilmis siparisler de iptal edilebiliyor |

---

## 3. Test Senaryolari ve Sonuclar

### 3.1 UNIT TEST - White Box (Ic Kod Testi)

> **Yaklasim:** Kaynak koda bakarak ic dallar, kosullar ve metodlarin detayli davranisi test edilir.

| Test ID | Test Adi | Beklenen | Gercek | Sonuc |
|---|---|---|---|---|
| TC-01 | `FiyatGuncelle_GecerliFiyat_BasariylaGuncellenir` | Fiyat 200 olmali | 200 ✓ | PASS |
| TC-02 | `FiyatGuncelle_NegatifFiyat_HataFirlatmali` | Exception firlatmali | Exception yok | FAIL |
| TC-03 | `StokAzalt_GecerliMiktar_DogruAzaltir` | Stok 7 olmali | 7 ✓ | PASS |
| TC-04 | `StokAzalt_StokuAsanMiktar_SifirdaKalmali` | Stok >= 0 olmali | Stok -10 oluyor | FAIL |
| TC-05 | `StokVarMi_StokSifir_YanlisDonerMi` | false doner | false ✓ | PASS |
| TC-06 | `UrunEkle_GecerliUrun_ToplamDogruHesaplanir` | Toplam 30000 TL | 30000 ✓ | PASS |
| TC-07 | `UrunEkle_SifirAdet_ArgumentExceptionFirlatmali` | Exception firlatmali | Exception firliyor ✓ | PASS |
| TC-08 | `IndirimliFiyatHesapla_YuzdeyuzdenFazlaIndirim_NegatifOlmamali` | Toplam >= 0 olmali | -7500 TL cikiyor | FAIL |
| TC-09 | `UrunEkle_AyniUrunIkiKez_AdediBirlesmeli` | OgeSayisi = 1 olmali | OgeSayisi = 2 oluyor | FAIL |
| TC-10 | `UrunCikar_MevcutUrun_Sepetten_Cikar` | OgeSayisi = 0 olmali | 0 ✓ | PASS |

---

### 3.2 BLACK BOX TEST (Davranis Testi)

> **Yaklasim:** Ic koddan bagimsiz, yalnizca girdi/cikti iliskisi test edilir.

| Test ID | Test Adi | Beklenen | Gercek | Sonuc |
|---|---|---|---|---|
| TC-BB-01 | `SiparisOlustur_GecerliSepet_DogruToplamlaGeri_Doner` | Siparis 16000 TL | 16000 TL ✓ | PASS |
| TC-BB-02 | `SiparisOlustur_BosSepet_HataFirlatmali` | Exception firlatmali | Exception yok | FAIL |
| TC-BB-03 | `OdemeYap_YetersizTutar_YanlisDonmeli` | false donmeli | true donuyor | FAIL |
| TC-BB-04 | `SiparisIptalEt_TeslimEdilmisSiparis_IptalEdilmemeli` | false donmeli | true donuyor | FAIL |
| TC-BB-05 | `SiparisIptalEt_OlmayanSiparis_YanlisDonerMi` | false donmeli | false ✓ | PASS |

---

### 3.3 GRAY BOX TEST (Durum Gecis Testi)

> **Yaklasim:** Kismi ic bilgi kullanilarak durum makinesi ve sinir degerleri test edilir.

| Test ID | Test Adi | Beklenen | Gercek | Sonuc |
|---|---|---|---|---|
| TC-GB-01 | `SiparisDurumu_OdemeSonrasi_OnayanlandiOlmali` | Durum = Onaylandi | Onaylandi ✓ | PASS |
| TC-GB-02 | `SiparisIptalEt_BeklemedekiSiparis_IptalEdildiOlmali` | Durum = IptalEdildi | IptalEdildi ✓ | PASS |
| TC-GB-03 | `UrunEkle_TamStokKadar_BasariliOlmali` | Toplam 15000 TL | 15000 TL ✓ | PASS |
| TC-GB-04 | `UrunEkle_StoktenFazla_HataFirlatmali` | Exception firlatmali (BUG-4) | Exception yok | FAIL |
| TC-GB-05 | `SiparisOlustur_SonraSepet_TemizlenmisOlmali` | OgeSayisi = 0 | 0 ✓ | PASS |

---

### 3.4 INTEGRATION TEST (Butunlesik Test)

> **Yaklasim:** Urun, Sepet ve SiparisServisi birlikte test edilir. Uctan uca senaryo dogrulanir.

| Test ID | Test Adi | Beklenen | Gercek | Sonuc |
|---|---|---|---|---|
| TC-INT-01 | `TamAkis_GecerliSenaryo_BasariylaTamamlanmali` | Tam akis basarili | Basarili ✓ | PASS |
| TC-INT-02 | `SiparisOlustur_SonraSepetBosOlmali` | OgeSayisi = 0 | 0 ✓ | PASS |
| TC-INT-03 | `CokluSiparis_HerSipariseBenzersizIdVerilmeli` | Farkli ID | Farkli ID ✓ | PASS |
| TC-INT-04 | `IndirimliFiyat_DogruUygulanmali` | 3600 TL | 3600 TL ✓ | PASS |
| TC-INT-05 | `TumSiparisleriGetir_TumSiparislerListelenmeli` | 2 siparis | 2 ✓ | PASS |

---

## 4. Ozet Tablo

| Test Turu | Toplam | PASS | FAIL |
|---|---|---|---|
| Unit Test (White Box) | 10 | 6 | 4 |
| Black Box Test | 5 | 2 | 3 |
| Gray Box Test | 5 | 4 | 1 |
| Integration Test | 5 | 5 | 0 |
| **TOPLAM** | **25** | **17** | **8** |

---

## 5. FAIL Olan Testlerin Detayli Analizi

### TC-02 - `FiyatGuncelle_NegatifFiyat_HataFirlatmali`
- **Neden Fail?** `Urun.FiyatGuncelle()` metodu herhangi bir dogrulama yapmiyor. `Fiyat = yeniFiyat` satiri negatif degerleri dogrudan kabul ediyor.
- **Bug:** BUG-1
- **Duzeltme:** `if (yeniFiyat < 0) throw new ArgumentException("Fiyat negatif olamaz.");`

---

### TC-04 - `StokAzalt_StokuAsanMiktar_SifirdaKalmali`
- **Neden Fail?** `StokMiktari -= miktar` islemi stok miktari kontrolu yapmadan uygulanıyor. Stok negatife (-10) dusebiliyor.
- **Bug:** BUG-2
- **Duzeltme:** `if (miktar > StokMiktari) throw new InvalidOperationException("Yetersiz stok.");`

---

### TC-08 - `IndirimliFiyatHesapla_YuzdeyuzdenFazlaIndirim_NegatifOlmamali`
- **Neden Fail?** `IndirimliFiyatHesapla(150)` cagirisinda indirim %150 olarak uygulaninca toplam negatif cikiyor (-7500 TL). Yuzde araligi [0,100] ile sinirlandirilmamis.
- **Bug:** BUG-5
- **Duzeltme:** `if (indirimYuzdesi < 0 || indirimYuzdesi > 100) throw new ArgumentOutOfRangeException(...);`

---

### TC-09 - `UrunEkle_AyniUrunIkiKez_AdediBirlesmeli`
- **Neden Fail?** `Sepet.UrunEkle()` her cagirida yeni bir `SepetOgesi` olusturuyor. Ayni `UrunId` icin mevcut ogeyi bulup adedi artirmiyor.
- **Bug:** BUG-3
- **Duzeltme:** Mevcut ogeyi bul, varsa Adet'i artir; yoksa yeni ekle.

---

### TC-BB-02 - `SiparisOlustur_BosSepet_HataFirlatmali`
- **Neden Fail?** `SiparisServisi.SiparisOlustur()` sepet bosluğunu kontrol etmiyor. Bos sepete 0 TL'lik siparis olusturuluyor.
- **Bug:** BUG-6
- **Duzeltme:** `if (!sepet.Ogeler.Any()) throw new InvalidOperationException("Sepet bos.");`

---

### TC-BB-03 - `OdemeYap_YetersizTutar_YanlisDonmeli`
- **Neden Fail?** `OdemeYap()` odeme miktarini (`odenenTutar`) hic kullanmiyor. Sadece odeme turunu kontrol ediyor ve her zaman `true` donduruyor.
- **Bug:** BUG-7
- **Duzeltme:** `if (odenenTutar < siparis.ToplamTutar) return false;`

---

### TC-BB-04 - `SiparisIptalEt_TeslimEdilmisSiparis_IptalEdilmemeli`
- **Neden Fail?** `SiparisIptalEt()` sipaриsin mevcut durumunu kontrol etmiyor. `TeslimEdildi` durumundaki siparis de `IptalEdildi` yapiliyor.
- **Bug:** BUG-8
- **Duzeltme:** `if (siparis.Durum == SiparisDurumu.TeslimEdildi) return false;`

---

### TC-GB-04 - `UrunEkle_StoktenFazla_HataFirlatmali`
- **Neden Fail?** `Sepet.UrunEkle()` metodu hic stok kontrolu yapmiyor. Stokta yalnizca 3 adet olan bir urunun 5 adedi kolayca sepete eklenebiliyor. Bu durum envanter tutarsizligina ve gercekte karsilanamayacak siparislere yol acar.
- **Bug:** BUG-4
- **Duzeltme:** `if (adet > urun.StokMiktari) throw new InvalidOperationException("Stoktan fazla eklenemez.");`

---

## 6. Sonuc

Bu proje, bilerek hatali yazilmis bir e-ticaret sisteminde dort farkli test metodolojisinin (White Box, Black Box, Gray Box, Integration) nasil uygulandigini gostermistir. Kaynak koda toplam **8 bug** bilerek yerlestirilmis olup **8 FAIL** testin tamami bu bug'lara birebir karsilik gelmektedir. Her bir hata yukarida kodu, nedeni ve duzeltme yontemiyle birlikte ayrintili sekilde belgelenmistir.
