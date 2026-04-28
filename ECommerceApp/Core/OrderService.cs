using System;
using System.Collections.Generic;

namespace ECommerceApp.Core
{
    public enum SiparisDurumu
    {
        Beklemede,
        Onaylandi,
        Kargolandi,
        TeslimEdildi,
        IptalEdildi
    }

    public enum OdemeTuru
    {
        KrediKarti,
        HavaleEFT,
        Nakit
    }

    public class Siparis
    {
        public int SiparisId { get; set; }
        public List<SepetOgesi> Ogeler { get; set; }
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
        public string MusteriAdi { get; set; }

        public Siparis(int siparisId, string musteriAdi, List<SepetOgesi> ogeler, decimal toplam)
        {
            SiparisId = siparisId;
            MusteriAdi = musteriAdi;
            Ogeler = ogeler;
            ToplamTutar = toplam;
            Durum = SiparisDurumu.Beklemede;
            OlusturulmaTarihi = DateTime.Now;
        }
    }

    public class SiparisServisi
    {
        private List<Siparis> _siparisler = new List<Siparis>();
        private int _sonrakiSiparisId = 1;

        // BUG #6: Bos sepete siparis verilebiliyor
        public Siparis SiparisOlustur(Sepet sepet, string musteriAdi)
        {
            // Sepet bos kontrolu yapilmiyor!
            decimal toplam = sepet.ToplamHesapla();
            var siparis = new Siparis(_sonrakiSiparisId++, musteriAdi, new List<SepetOgesi>(sepet.Ogeler), toplam);
            _siparisler.Add(siparis);
            sepet.Temizle();
            return siparis;
        }

        // BUG #7: Odeme dogrulamasi yok - her tutar icin "basarili" doner
        public bool OdemeYap(Siparis siparis, OdemeTuru odemeTuru, decimal odenenTutar)
        {
            // odenenTutar < siparis.ToplamTutar olsa bile true doner!
            if (odemeTuru == OdemeTuru.KrediKarti || odemeTuru == OdemeTuru.HavaleEFT || odemeTuru == OdemeTuru.Nakit)
            {
                siparis.Durum = SiparisDurumu.Onaylandi;
                return true;
            }
            return false;
        }

        public bool SiparisIptalEt(int siparisId)
        {
            var siparis = _siparisler.Find(s => s.SiparisId == siparisId);
            if (siparis == null) return false;

            // BUG #8: Teslim edilmis siparisler de iptal edilebiliyor
            siparis.Durum = SiparisDurumu.IptalEdildi; // BUG #8
            return true;
        }

        public Siparis SiparisGetir(int siparisId)
        {
            return _siparisler.Find(s => s.SiparisId == siparisId);
        }

        public List<Siparis> TumSiparisleriGetir()
        {
            return _siparisler;
        }
    }
}
