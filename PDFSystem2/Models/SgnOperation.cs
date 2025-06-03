using System;

namespace PDFSystem2.Models
{
    public class SgnOperation
    {
        public int ID { get; set; }
        public int SGN_CIRCULAR_ID { get; set; }
        public string OPERATION_TYPE { get; set; }
        public string OPERATION_CODE { get; set; }
        public string ACIKLAMA { get; set; }
        public bool AKTIF_PASIF { get; set; }
        public DateTime KAYIT_TARIHI { get; set; }
    }
} 