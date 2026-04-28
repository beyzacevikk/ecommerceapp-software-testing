using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests
{
    /// <summary>
    /// INTEGRATION TEST - Birden fazla bileseni birlikte test eder.
    /// Urun + Sepet + SiparisServisi entegrasyonu.
    /// </summary>
    [TestFixture]
    public class ETicaretEntegrasyonTestleri
    {
        private SiparisServisi _siparisServisi;
        private Sepet _sepet;

        [SetUp]
        public void KurulumYap()
        {
            _siparisServisi = new SiparisServisi();
            _sepet = new Sepet();
        }

        // TC-INT-01: Tam alisveris akisi (mutlu yol)
        [Test]
        public void TamAkis_GecerliSenaryo_BasariylaTamamlanmali()
        {
            // Arrange
            var laptop = new Urun(1, "Laptop", 20000m, 5);
            var fare = new Urun(2, "Fare", 500m, 20);

            // Sepete ekle
            _sepet.UrunEkle(laptop, 1);
            _sepet.UrunEkle(fare, 2);

            Assert.That(_sepet.ToplamHesapla(), Is.EqualTo(21000m));

            // Siparis ver
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Ayse Demir");

            Assert.That(siparis, Is.Not.Null);
            Assert.That(siparis.ToplamTutar, Is.EqualTo(21000m));
            Assert.That(siparis.Durum, Is.EqualTo(SiparisDurumu.Beklemede));

            // Odeme yap
            bool odemeBasarili = _siparisServisi.OdemeYap(
                siparis, OdemeTuru.KrediKarti, 21000m);

            Assert.That(odemeBasarili, Is.True);
            Assert.That(siparis.Durum, Is.EqualTo(SiparisDurumu.Onaylandi));
        }

        // TC-INT-02: Siparis sonrasi sepet bosaliyor mu?
        [Test]
        public void SiparisOlustur_SonraSepetBosOlmali()
        {
            var urun = new Urun(3, "Klavye", 1500m, 10);
            _sepet.UrunEkle(urun, 2);

            _siparisServisi.SiparisOlustur(_sepet, "Mehmet Can");

            Assert.That(_sepet.OgeSayisi, Is.EqualTo(0),
                "Siparis sonrasi sepet temizlenmeli.");
        }

        // TC-INT-03: Coklu siparis - her siparis ayri ID almali
        [Test]
        public void CokluSiparis_HerSipariseBenzersizIdVerilmeli()
        {
            var urun = new Urun(4, "Tablet", 10000m, 5);

            _sepet.UrunEkle(urun, 1);
            var siparis1 = _siparisServisi.SiparisOlustur(_sepet, "Musteri A");

            _sepet.UrunEkle(urun, 1);
            var siparis2 = _siparisServisi.SiparisOlustur(_sepet, "Musteri B");

            Assert.That(siparis1.SiparisId, Is.Not.EqualTo(siparis2.SiparisId));
        }

        // TC-INT-04: Indirimli siparis entegrasyonu
        [Test]
        public void IndirimliFiyat_DogruUygulanmali()
        {
            var urun = new Urun(5, "Kulaklik", 2000m, 15);
            _sepet.UrunEkle(urun, 2); // 4000 TL

            decimal indirimliFiyat = _sepet.IndirimliFiyatHesapla(10); // %10 indirim -> 3600 TL

            Assert.That(indirimliFiyat, Is.EqualTo(3600m));
        }

        // TC-INT-05: Tum siparisler listesi
        [Test]
        public void TumSiparisleriGetir_TumSiparislerListelenmeli()
        {
            var u1 = new Urun(6, "Urun A", 100m, 10);
            var u2 = new Urun(7, "Urun B", 200m, 10);

            _sepet.UrunEkle(u1, 1);
            _siparisServisi.SiparisOlustur(_sepet, "Musteri 1");

            _sepet.UrunEkle(u2, 1);
            _siparisServisi.SiparisOlustur(_sepet, "Musteri 2");

            var tumSiparisler = _siparisServisi.TumSiparisleriGetir();
            Assert.That(tumSiparisler.Count, Is.EqualTo(2));
        }
    }
}
