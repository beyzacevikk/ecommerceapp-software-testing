using System;
using ECommerceApp.Core;

namespace ECommerceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== E-Ticaret Sistemi ===\n");

            // Urun olustur
            var dizustu = new Urun(1, "Dizustu Bilgisayar", 15000m, 5);
            var telefon = new Urun(2, "Telefon", 8000m, 10);

            Console.WriteLine($"Urunler: {dizustu.UrunAdi} ({dizustu.Fiyat} TL), {telefon.UrunAdi} ({telefon.Fiyat} TL)");

            // Sepete ekle
            var sepet = new Sepet();
            sepet.UrunEkle(dizustu, 1);
            sepet.UrunEkle(telefon, 2);

            Console.WriteLine($"\nSepet Toplami: {sepet.ToplamHesapla()} TL");
            Console.WriteLine($"%10 Indirimli: {sepet.IndirimliFiyatHesapla(10)} TL");

            // Siparis ver
            var siparisServisi = new SiparisServisi();
            var siparis = siparisServisi.SiparisOlustur(sepet, "Ahmet Yilmaz");
            Console.WriteLine($"\nSiparis #{siparis.SiparisId} olusturuldu. Durum: {siparis.Durum}");

            // Odeme yap
            bool odemeBasarili = siparisServisi.OdemeYap(siparis, OdemeTuru.KrediKarti, siparis.ToplamTutar);
            Console.WriteLine($"Odeme basarili: {odemeBasarili}, Siparis Durumu: {siparis.Durum}");

            Console.WriteLine("\nProgram tamamlandi.");
        }
    }
}
