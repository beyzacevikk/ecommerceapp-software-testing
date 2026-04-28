using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    /// <summary>
    /// WHITE BOX TEST - Ic kodun mantigini ve dallarini test eder.
    /// Kaynak koda erisim varsayilir.
    /// </summary>
    [TestFixture]
    public class UrunBirimTestleri
    {
        private Urun _urun;

        [SetUp]
        public void KurulumYap()
        {
            _urun = new Urun(1, "Test Urun", 100m, 10);
        }

        // TC-01: Gecerli fiyat guncelleme
        [Test]
        public void FiyatGuncelle_GecerliFiyat_BasariylaGuncellenir()
        {
            _urun.FiyatGuncelle(200m);
            Assert.That(_urun.Fiyat, Is.EqualTo(200m));
        }

        // TC-02: Negatif fiyat - BUG #1 yakalanir (FAIL beklenir)
        [Test]
        public void FiyatGuncelle_NegatifFiyat_HataFirlatmali()
        {
            // BUG #1: FiyatGuncelle negatif degeri kabul ediyor, exception firlatmiyor
            Assert.Throws<System.ArgumentException>(() => _urun.FiyatGuncelle(-50m),
                "Negatif fiyat kabul edilmemeli!");
        }

        // TC-03: Stok azaltma gecerli
        [Test]
        public void StokAzalt_GecerliMiktar_DogruAzaltir()
        {
            _urun.StokAzalt(3);
            Assert.That(_urun.StokMiktari, Is.EqualTo(7));
        }

        // TC-04: Stoktan fazla azaltma - BUG #2 yakalanir (FAIL beklenir)
        [Test]
        public void StokAzalt_StokuAsanMiktar_SifirdaKalmali()
        {
            // BUG #2: Stok negatife dusuyor, kontrol yok
            _urun.StokAzalt(20);
            Assert.That(_urun.StokMiktari, Is.GreaterThanOrEqualTo(0),
                "Stok negatif olamaz!");
        }

        // TC-05: StokVarMi stok sifirda false doner
        [Test]
        public void StokVarMi_StokSifir_YanlisDonerMi()
        {
            var stoksuzmUrun = new Urun(2, "Tukenen Urun", 50m, 0);
            Assert.That(stoksuzmUrun.StokVarMi(), Is.False);
        }
    }

    [TestFixture]
    public class SepetBirimTestleri
    {
        private Sepet _sepet;
        private Urun _urun;

        [SetUp]
        public void KurulumYap()
        {
            _sepet = new Sepet();
            _urun = new Urun(1, "Laptop", 15000m, 5);
        }

        // TC-06: Sepete urun ekleme ve toplam hesaplama
        [Test]
        public void UrunEkle_GecerliUrun_ToplamDogruHesaplanir()
        {
            _sepet.UrunEkle(_urun, 2);
            Assert.That(_sepet.ToplamHesapla(), Is.EqualTo(30000m));
        }

        // TC-07: Sifir adet ekleme exception firlatmali
        [Test]
        public void UrunEkle_SifirAdet_ArgumentExceptionFirlatmali()
        {
            Assert.Throws<System.ArgumentException>(() => _sepet.UrunEkle(_urun, 0));
        }

        // TC-08: %100'den fazla indirim -> negatif toplam - BUG #5 yakalanir (FAIL beklenir)
        [Test]
        public void IndirimliFiyatHesapla_YuzdeyuzdenFazlaIndirim_NegatifOlmamali()
        {
            _sepet.UrunEkle(_urun, 1);
            decimal sonuc = _sepet.IndirimliFiyatHesapla(150); // %150 indirim
            // BUG #5: Sonuc negatif cikiyor
            Assert.That(sonuc, Is.GreaterThanOrEqualTo(0),
                "Indirim uygulaninca toplam negatif olmamali!");
        }

        // TC-09: Ayni urun 2 kez eklenince birlesme - BUG #3 (FAIL beklenir)
        [Test]
        public void UrunEkle_AyniUrunIkiKez_AdediBirlesmeli()
        {
            _sepet.UrunEkle(_urun, 1);
            _sepet.UrunEkle(_urun, 1);
            // BUG #3: 2 ayri SepetOgesi olusturuyor, birlesmiyor
            Assert.That(_sepet.OgeSayisi, Is.EqualTo(1),
                "Ayni urun sepette tek satir olmali!");
        }

        // TC-10: Urun cikartma
        [Test]
        public void UrunCikar_MevcutUrun_Sepetten_Cikar()
        {
            _sepet.UrunEkle(_urun, 1);
            _sepet.UrunCikar(_urun.UrunId);
            Assert.That(_sepet.OgeSayisi, Is.EqualTo(0));
        }
    }
}
