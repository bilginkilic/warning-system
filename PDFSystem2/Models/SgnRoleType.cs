using System;

namespace PDFSystem2.Models
{
    public class SgnRoleType
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string ROLE_GROUP { get; set; }
        public string ROLE_TYPE { get; set; }
        public int MIN_SIGNATURE_COUNT { get; set; }
        public int? MAX_SIGNATURE_COUNT { get; set; }
        public decimal? YETKI_LIMITI { get; set; }
        public string PARA_BIRIMI { get; set; }
        public string ACIKLAMA { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }
} 