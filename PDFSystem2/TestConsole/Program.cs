using System;
using System.Linq;

namespace PDFSystem2.TestConsole
{
    // Simplified models for testing (without dependency issues)
    public class SgnCircular
    {
        public int ID { get; set; }
        public string FIRMA_UNVANI { get; set; }
        public string FIRMA_HESAP_NUMARASI { get; set; }
        public DateTime? IMZA_SIRKULERI_DUZENLEME_TARIHI { get; set; }
        public DateTime? IMZA_SIRKULERI_GECERLILIK_TARIHI { get; set; }
        public bool SURESIZ_GECERLI { get; set; }
        public string OZEL_DURUMLAR { get; set; }
        public string NOTER_IMZA_SIRKULERI_NO { get; set; }
        public string KULLANICI { get; set; }
        public string ACIKLAMA { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }

    public class SgnCircularDetail
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string ADI_SOYADI { get; set; }
        public string YETKI_SEKLI { get; set; }
        public string YETKI_SURE { get; set; }
        public string IMZA_YETKI_GRUBU { get; set; }
        public bool SINIRLI_YETKI_VAR_MI { get; set; }
        public string YETKI_OLDUGU_ISLEMLER { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }

    public class SgnOperation
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string OPERATION_TYPE { get; set; }
        public string OPERATION_CODE { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }

    public class SgnRoleType
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string ROLE_GROUP { get; set; }
        public string ROLE_TYPE { get; set; }
        public int MIN_SIGNATURE_COUNT { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }

    // Simplified MockDataService for testing
    public class SimpleMockDataService
    {
        public static void TestData()
        {
            Console.WriteLine("=== İMZA SİRKÜLERİ YÖNETİM SİSTEMİ TEST ===\n");

            // Test Circular Data
            var circular = new SgnCircular
            {
                ID = 1,
                FIRMA_UNVANI = "şirket aş",
                FIRMA_HESAP_NUMARASI = "9999",
                IMZA_SIRKULERI_DUZENLEME_TARIHI = new DateTime(2023, 12, 20),
                IMZA_SIRKULERI_GECERLILIK_TARIHI = new DateTime(2024, 12, 20),
                SURESIZ_GECERLI = false,
                OZEL_DURUMLAR = "Sırülerde süre belirilmemiş ve süresiz geçerli işlemmiş işe imza sırküleri geçerlilik tarihi 30 yıl sonra olacak şekilde girinmektedir.",
                NOTER_IMZA_SIRKULERI_NO = "30903",
                KULLANICI = "Kullanıcı için TEXT",
                ACIKLAMA = "Açıklama alanı olarak görülecektir.",
                AKTIF_PASIF = true,
                KAYIT_TARIHI = DateTime.Now
            };

            Console.WriteLine("🏢 FİRMA BİLGİLERİ:");
            Console.WriteLine(string.Format("   Firma Unvanı: {0}", circular.FIRMA_UNVANI));
            Console.WriteLine(string.Format("   Hesap No: {0}", circular.FIRMA_HESAP_NUMARASI));
            Console.WriteLine(string.Format("   Düzenlenme Tarihi: {0:dd/MM/yyyy}", circular.IMZA_SIRKULERI_DUZENLEME_TARIHI));
            Console.WriteLine(string.Format("   Geçerlilik Tarihi: {0:dd/MM/yyyy}", circular.IMZA_SIRKULERI_GECERLILIK_TARIHI));
            Console.WriteLine(string.Format("   Noter Sırküler No: {0}", circular.NOTER_IMZA_SIRKULERI_NO));
            Console.WriteLine();

            // Test Detail Data
            var details = new[]
            {
                new SgnCircularDetail
                {
                    ID = 1,
                    SGN_CIRCULAR_ID = 1,
                    ADI_SOYADI = "ahmet",
                    YETKI_SEKLI = "MÜŞTEREKİN",
                    YETKI_SURE = "10/4/2025",
                    IMZA_YETKI_GRUBU = "A",
                    SINIRLI_YETKI_VAR_MI = true,
                    YETKI_OLDUGU_ISLEMLER = "C.İLE BİRLİKTE (futur sınırlamaları varsa) - İnternet Bankacılığı Sözleşmeleri - Kredi Sözleşmeleri",
                    AKTIF_PASIF = true,
                    KAYIT_TARIHI = DateTime.Now
                },
                new SgnCircularDetail
                {
                    ID = 2,
                    SGN_CIRCULAR_ID = 1,
                    ADI_SOYADI = "memo",
                    YETKI_SEKLI = "MÜNFERİDEN",
                    YETKI_SURE = "Aksi Karar alınıncaya kadar",
                    IMZA_YETKI_GRUBU = "B",
                    SINIRLI_YETKI_VAR_MI = true,
                    YETKI_OLDUGU_ISLEMLER = "A.İLE BİRLİKTE (futur sınırlamaları varsa yazılır)",
                    AKTIF_PASIF = true,
                    KAYIT_TARIHI = DateTime.Now
                },
                new SgnCircularDetail
                {
                    ID = 3,
                    SGN_CIRCULAR_ID = 1,
                    ADI_SOYADI = "tankut",
                    YETKI_SEKLI = "SINIRLI YETKİLİ(İÇ YÖNERGEDEKİ BELİRTİLDİĞİ ŞEKİLDE)",
                    IMZA_YETKI_GRUBU = "C",
                    SINIRLI_YETKI_VAR_MI = false,
                    YETKI_OLDUGU_ISLEMLER = "A.İLE BİRLİKTE",
                    AKTIF_PASIF = true,
                    KAYIT_TARIHI = DateTime.Now
                }
            };

            Console.WriteLine("👥 YETKİLİ BİLGİLERİ:");
            foreach (var detail in details)
            {
                Console.WriteLine(string.Format("   • {0} ({1} GRUBU)", detail.ADI_SOYADI, detail.IMZA_YETKI_GRUBU));
                Console.WriteLine(string.Format("     Yetki Şekli: {0}", detail.YETKI_SEKLI));
                Console.WriteLine(string.Format("     Yetki Süresi: {0}", detail.YETKI_SURE));
                Console.WriteLine(string.Format("     Sınırlı Yetki: {0}", (detail.SINIRLI_YETKI_VAR_MI ? "Var" : "Yok")));
                Console.WriteLine();
            }

            // Test Operations
            var operations = new[]
            {
                "Kredi Sözleşmeleri",
                "Türev sözleşmeleri",
                "Transfer işlemleri",
                "Eft işlemleri", 
                "Döviz işlemleri",
                "İnternet Bankacılığı İşlemleri"
            };

            Console.WriteLine("📋 İŞLEM TÜRLERİ:");
            for (int i = 0; i < operations.Length; i++)
            {
                Console.WriteLine(string.Format("   {0}. {1}", i + 1, operations[i]));
            }
            Console.WriteLine();

            // Test Role Types
            var roleTypes = new[]
            {
                ("A GRUBU", "1. DERECE İMZA YETKİLİSİ"),
                ("B GRUBU", "2. DERECE İMZA YETKİLİSİ"),
                ("C GRUBU", "3. DERECE İMZA YETKİLİSİ"),
                ("C1 GRUBU", "4. DERECE İMZA YETKİLİSİ"),
                ("C2 GRUBU", ""),
                ("D GRUBU", ""),
                ("E GRUBU", ""),
                ("K GRUBU", ""),
                ("F GRUBU", "")
            };

            Console.WriteLine("🏆 YETKİ TÜRLERİ:");
            foreach (var roleType in roleTypes)
            {
                Console.WriteLine(string.Format("   • {0}: {1}", roleType.Item1, roleType.Item2));
            }
            Console.WriteLine();

            Console.WriteLine("✅ Test verisi başarıyla yüklendi!");
            Console.WriteLine("📊 Sistem hazır! Ana form üzerinden görsel arayüz ile çalışabilir.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            try
            {
                SimpleMockDataService.TestData();
                
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("🎯 PDFSystem2 - İmza Sirküleri Yönetim Sistemi");
                Console.WriteLine("📝 Mock Data Testi Tamamlandı");
                Console.WriteLine("🔗 Gizmox WebGUI ile tam özellikli web arayüzü mevcut");
                Console.WriteLine(new string('=', 60));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("❌ Hata: {0}", ex.Message));
            }
            
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
    }
} 