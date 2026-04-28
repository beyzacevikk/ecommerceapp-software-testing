using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    /// <summary>
    /// BLACK BOX TEST - Sadece girdi/cikti davranisi test edilir.
    /// Ic implementasyon bilinmez / onemsenmez.
    /// </summary>
    [TestFixture]
    public class KaraSutunTestleri
    {
        private SiparisServisi _siparisServisi;
        private Sepet _sepet;
        private Urun _urun;

        [SetUp]
        public void KurulumYap()
        {
            _siparisServisi = new SiparisServisi();
            _sepet = new Sepet();
            _urun = new Urun(1, "Telefon", 8000m, 10);
        }

        // TC-BB-01: Normal siparis akisi - gecerli girdi -> gecerli cikti
        [Test]
        public void SiparisOlustur_GecerliSepet_DogruToplamlaGeri_Doner()
        {
            _sepet.UrunEkle(_urun, 2);
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Zeynep Kaya");

            Assert.That(siparis, Is.Not.Null);
            Assert.That(siparis.ToplamTutar, Is.EqualTo(16000m));
            Assert.That(siparis.MusteriAdi, Is.EqualTo("Zeynep Kaya"));
        }

        // TC-BB-02: Bos sepete siparis - BUG #6 yakalanir (FAIL beklenir)
        [Test]
        public void SiparisOlustur_BosSepet_HataFirlatmali()
        {
            // BUG #6: Bos sepete siparis aliniyor, hata yok
            Assert.Throws<System.InvalidOperationException>(
                () => _siparisServisi.SiparisOlustur(_sepet, "Ali Veli"),
                "Bos sepete siparis verilemez!");
        }

        // TC-BB-03: Eksik odeme yine de onaylaniyor - BUG #7 (FAIL beklenir)
        [Test]
        public void OdemeYap_YetersizTutar_YanlisDonmeli()
        {
            _sepet.UrunEkle(_urun, 1);
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Musteri");

            // 1 TL oduyor, 8000 TL borcu var
            bool sonuc = _siparisServisi.OdemeYap(siparis, OdemeTuru.KrediKarti, 1m);

            // BUG #7: Yetersiz odeme yine true donuyor
            Assert.That(sonuc, Is.False,
                "Eksik odeme onaylanmamali!");
        }

        // TC-BB-04: Teslim edilmis siparis iptal edilmemeli - BUG #8 (FAIL beklenir)
        [Test]
        public void SiparisIptalEt_TeslimEdilmisSiparis_IptalEdilmemeli()
        {
            _sepet.UrunEkle(_urun, 1);
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Musteri");
            siparis.Durum = SiparisDurumu.TeslimEdildi;

            bool iptalEdildi = _siparisServisi.SiparisIptalEt(siparis.SiparisId);

            // BUG #8: Teslim edilmis siparis iptal edilebiliyor
            Assert.That(iptalEdildi, Is.False,
                "Teslim edilmis siparis iptal edilemez!");
        }

        // TC-BB-05: Olmayan siparis iptali false doner
        [Test]
        public void SiparisIptalEt_OlmayanSiparis_YanlisDonerMi()
        {
            bool sonuc = _siparisServisi.SiparisIptalEt(9999);
            Assert.That(sonuc, Is.False);
        }
    }

    /// <summary>
    /// GRAY BOX TEST - Kismi ic bilgi kullanilir.
    /// Durum gecisleri ve sinir degerleri test edilir.
    /// </summary>
    [TestFixture]
    public class GriSutunTestleri
    {
        private SiparisServisi _siparisServisi;
        private Sepet _sepet;
        private Urun _urun;

        [SetUp]
        public void KurulumYap()
        {
            _siparisServisi = new SiparisServisi();
            _sepet = new Sepet();
            _urun = new Urun(1, "Monitor", 5000m, 3);
        }

        // TC-GB-01: Siparis durumu Beklemede -> Onaylandi gecisi
        [Test]
        public void SiparisDurumu_OdemeSonrasi_OnayanlandiOlmali()
        {
            _sepet.UrunEkle(_urun, 1);
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Musteri");
            Assert.That(siparis.Durum, Is.EqualTo(SiparisDurumu.Beklemede));

            _siparisServisi.OdemeYap(siparis, OdemeTuru.KrediKarti, 5000m);
            Assert.That(siparis.Durum, Is.EqualTo(SiparisDurumu.Onaylandi));
        }

        // TC-GB-02: Siparis iptal -> durum IptalEdildi olmali
        [Test]
        public void SiparisIptalEt_BeklemedekiSiparis_IptalEdildiOlmali()
        {
            _sepet.UrunEkle(_urun, 1);
            var siparis = _siparisServisi.SiparisOlustur(_sepet, "Musteri");

            _siparisServisi.SiparisIptalEt(siparis.SiparisId);
            Assert.That(siparis.Durum, Is.EqualTo(SiparisDurumu.IptalEdildi));
        }

        // TC-GB-03: Stok sinirinda urun ekleme - boundary test
        [Test]
        public void UrunEkle_TamStokKadar_BasariliOlmali()
        {
            // Urun stoku 3, tam 3 adet ekle
            _sepet.UrunEkle(_urun, 3);
            Assert.That(_sepet.ToplamHesapla(), Is.EqualTo(15000m));
        }

        // TC-GB-04: Stoktan fazla urun eklenmemeli
        [Test]
        public void UrunEkle_StoktenFazla_HataFirlatmali()
        {
            // Stok 3 ama 5 eklenmeye calisiliyor
            // BUG: Stok kontrolu yok, ekleniyor
            Assert.Throws<System.InvalidOperationException>(
                () => _sepet.UrunEkle(_urun, 5),
                "Stoktan fazla eklenemez!");
        }

        // TC-GB-05: Siparis sonrasi sepet temizlenmeli
        [Test]
        public void SiparisOlustur_SonraSepet_TemizlenmisOlmali()
        {
            _sepet.UrunEkle(_urun, 1);
            _siparisServisi.SiparisOlustur(_sepet, "Musteri");
            Assert.That(_sepet.OgeSayisi, Is.EqualTo(0));
        }
    }
}
