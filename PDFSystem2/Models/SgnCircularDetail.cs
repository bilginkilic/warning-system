using System;

namespace PDFSystem2.Models
{
    public class SgnCircularDetail
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string ADI_SOYADI { get; set; }
        public string YETKI_SEKLI { get; set; }
        public string YETKI_SURE { get; set; }
        public DateTime? YETKI_BITIS_TARIHI { get; set; }
        public string IMZA_YETKI_GRUBU { get; set; }
        public bool SINIRLI_YETKI_VAR_MI { get; set; }
        public string YETKI_OLDUGU_ISLEMLER { get; set; }
        public string IMZA_GORUNTUSU { get; set; }
        public int? IMZA_KOORDINAT_X { get; set; }
        public int? IMZA_KOORDINAT_Y { get; set; }
        public int? IMZA_KOORDINAT_WIDTH { get; set; }
        public int? IMZA_KOORDINAT_HEIGHT { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }
} 