using System;

namespace PDFSystem2.Models
{
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
        public string IMZA_SIRKULERI_GORUNTUSU { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public string ACIKLAMA { get; set; }
        public string KULLANICI { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
        public DateTime? GUNCELLEME_TARIHI { get; set; }
    }
} 