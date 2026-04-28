namespace ECommerceApp.Core
{
    public class Urun
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int StokMiktari { get; set; }

        public Urun(int urunId, string urunAdi, decimal fiyat, int stokMiktari)
        {
            UrunId = urunId;
            UrunAdi = urunAdi;
            Fiyat = fiyat;
            StokMiktari = stokMiktari;
        }

        // BUG #1: Negatif fiyata izin veriyor, kontrol yok
        public void FiyatGuncelle(decimal yeniFiyat)
        {
            Fiyat = yeniFiyat; // Negatif fiyat set edilebilir!
        }

        // BUG #2: Stok azaltma sinir kontrolu yok
        public void StokAzalt(int miktar)
        {
            StokMiktari -= miktar; // Stok negatife dusebilir!
        }

        public bool StokVarMi()
        {
            return StokMiktari > 0;
        }
    }
}
