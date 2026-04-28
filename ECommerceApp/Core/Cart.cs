using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class SepetOgesi
    {
        public Urun Urun { get; set; }
        public int Adet { get; set; }

        public SepetOgesi(Urun urun, int adet)
        {
            Urun = urun;
            Adet = adet;
        }

        public decimal ToplamFiyat => Urun.Fiyat * Adet;
    }

    public class Sepet
    {
        private List<SepetOgesi> _ogeler = new List<SepetOgesi>();

        public IReadOnlyList<SepetOgesi> Ogeler => _ogeler.AsReadOnly();

        // BUG #3: Ayni urun tekrar eklenince duplicate olusturuyor, adet artmiyor
        public void UrunEkle(Urun urun, int adet)
        {
            if (adet <= 0)
                throw new System.ArgumentException("Adet pozitif olmalidir.");

            // Stok kontrolu yok! Stoktan fazla eklenebilir
            _ogeler.Add(new SepetOgesi(urun, adet));
        }

        public void UrunCikar(int urunId)
        {
            var oge = _ogeler.FirstOrDefault(o => o.Urun.UrunId == urunId);
            if (oge != null)
                _ogeler.Remove(oge);
        }

        // BUG #4: Sepet boskken toplam 0 donmeli ama indirim uygulandiginda hatali hesapliyor
        public decimal ToplamHesapla()
        {
            return _ogeler.Sum(o => o.ToplamFiyat);
        }

        // BUG #5: Indirim %100'den fazla olabilir -> negatif toplam
        public decimal IndirimliFiyatHesapla(decimal indirimYuzdesi)
        {
            decimal toplam = ToplamHesapla();
            return toplam - (toplam * indirimYuzdesi / 100);
        }

        public int OgeSayisi => _ogeler.Count;

        public void Temizle()
        {
            _ogeler.Clear();
        }
    }
}
