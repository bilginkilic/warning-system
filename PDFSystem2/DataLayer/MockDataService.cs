using System;
using System.Collections.Generic;
using System.Linq;
using PDFSystem2.Models;

namespace PDFSystem2.DataLayer
{
    public class MockDataService
    {
        private static List<SgnCircular> _circulars = new List<SgnCircular>();
        private static List<SgnCircularDetail> _circularDetails = new List<SgnCircularDetail>();
        private static List<SgnOperation> _operations = new List<SgnOperation>();
        private static List<SgnRoleType> _roleTypes = new List<SgnRoleType>();
        
        private static int _nextCircularId = 1;
        private static int _nextDetailId = 1;
        private static int _nextOperationId = 1;
        private static int _nextRoleTypeId = 1;

        static MockDataService()
        {
            InitializeMockData();
        }

        private static void InitializeMockData()
        {
            // Mock Circular Data
            var circular1 = new SgnCircular
            {
                ID = _nextCircularId++,
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
            _circulars.Add(circular1);

            // Mock Detail Data
            var detail1 = new SgnCircularDetail
            {
                ID = _nextDetailId++,
                SGN_CIRCULAR_ID = circular1.ID,
                ADI_SOYADI = "ahmet",
                YETKI_SEKLI = "MÜŞTEREKİN",
                YETKI_SURE = "10/4/2025",
                YETKI_BITIS_TARIHI = new DateTime(2025, 4, 10),
                IMZA_YETKI_GRUBU = "A",
                SINIRLI_YETKI_VAR_MI = true,
                YETKI_OLDUGU_ISLEMLER = "C.İLE BİRLİKTE (futur sınırlamaları varsa) - İnternet Bankacılığı Sözleşmeleri - Kredi Sözleşmeleri - Transfer İşlemleri TL&YP - Türev İşlemler",
                AKTIF_PASIF = true,
                KAYIT_TARIHI = DateTime.Now
            };
            _circularDetails.Add(detail1);

            var detail2 = new SgnCircularDetail
            {
                ID = _nextDetailId++,
                SGN_CIRCULAR_ID = circular1.ID,
                ADI_SOYADI = "memo",
                YETKI_SEKLI = "MÜNFERİDEN",
                YETKI_SURE = "Aksi Karar alınıncaya kadar",
                IMZA_YETKI_GRUBU = "B",
                SINIRLI_YETKI_VAR_MI = true,
                YETKI_OLDUGU_ISLEMLER = "A.İLE BİRLİKTE (futur sınırlamaları varsa yazılır)",
                AKTIF_PASIF = true,
                KAYIT_TARIHI = DateTime.Now
            };
            _circularDetails.Add(detail2);

            var detail3 = new SgnCircularDetail
            {
                ID = _nextDetailId++,
                SGN_CIRCULAR_ID = circular1.ID,
                ADI_SOYADI = "tankut",
                YETKI_SEKLI = "SINIRLI YETKİLİ(İÇ YÖNERGEDEKİ BELİRTİLDİĞİ ŞEKİLDE)",
                IMZA_YETKI_GRUBU = "C",
                SINIRLI_YETKI_VAR_MI = false,
                YETKI_OLDUGU_ISLEMLER = "A.İLE BİRLİKTE",
                AKTIF_PASIF = true,
                KAYIT_TARIHI = DateTime.Now
            };
            _circularDetails.Add(detail3);

            // Mock Operation Data
            var operations = new List<string>
            {
                "Kredi Sözleşmeleri",
                "Türev sözleşmeleri", 
                "Transfer işlemleri",
                "Eft işlemleri",
                "Döviz işlemleri",
                "İnternet Bankacılığı İşlemleri"
            };

            foreach (var operation in operations)
            {
                _operations.Add(new SgnOperation
                {
                    ID = _nextOperationId++,
                    SGN_CIRCULAR_ID = circular1.ID,
                    OPERATION_TYPE = operation,
                    OPERATION_CODE = _nextOperationId.ToString(),
                    AKTIF_PASIF = true,
                    KAYIT_TARIHI = DateTime.Now
                });
            }

            // Mock Role Type Data
            var roleTypes = new List<(string group, string type)>
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

            foreach (var roleType in roleTypes)
            {
                _roleTypes.Add(new SgnRoleType
                {
                    ID = _nextRoleTypeId++,
                    SGN_CIRCULAR_ID = circular1.ID,
                    ROLE_GROUP = roleType.group,
                    ROLE_TYPE = roleType.type,
                    MIN_SIGNATURE_COUNT = 1,
                    AKTIF_PASIF = true,
                    KAYIT_TARIHI = DateTime.Now
                });
            }
        }

        // SGN_CIRCULAR Methods
        public List<SgnCircular> GetAllCirculars()
        {
            return _circulars.Where(c => c.AKTIF_PASIF).ToList();
        }

        public SgnCircular GetCircularById(int id)
        {
            return _circulars.FirstOrDefault(c => c.ID == id && c.AKTIF_PASIF);
        }

        public int InsertCircular(SgnCircular circular)
        {
            circular.ID = _nextCircularId++;
            circular.KAYIT_TARIHI = DateTime.Now;
            circular.AKTIF_PASIF = true;
            _circulars.Add(circular);
            return circular.ID;
        }

        public bool UpdateCircular(SgnCircular circular)
        {
            var existing = _circulars.FirstOrDefault(c => c.ID == circular.ID);
            if (existing != null)
            {
                existing.FIRMA_UNVANI = circular.FIRMA_UNVANI;
                existing.FIRMA_HESAP_NUMARASI = circular.FIRMA_HESAP_NUMARASI;
                existing.IMZA_SIRKULERI_DUZENLEME_TARIHI = circular.IMZA_SIRKULERI_DUZENLEME_TARIHI;
                existing.IMZA_SIRKULERI_GECERLILIK_TARIHI = circular.IMZA_SIRKULERI_GECERLILIK_TARIHI;
                existing.SURESIZ_GECERLI = circular.SURESIZ_GECERLI;
                existing.OZEL_DURUMLAR = circular.OZEL_DURUMLAR;
                existing.NOTER_IMZA_SIRKULERI_NO = circular.NOTER_IMZA_SIRKULERI_NO;
                existing.IMZA_SIRKULERI_GORUNTUSU = circular.IMZA_SIRKULERI_GORUNTUSU;
                existing.ACIKLAMA = circular.ACIKLAMA;
                existing.KULLANICI = circular.KULLANICI;
                existing.GUNCELLEME_TARIHI = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool DeleteCircular(int id)
        {
            var circular = _circulars.FirstOrDefault(c => c.ID == id);
            if (circular != null)
            {
                circular.AKTIF_PASIF = false;
                circular.GUNCELLEME_TARIHI = DateTime.Now;
                return true;
            }
            return false;
        }

        // SGN_CIRCULARDETAIL Methods
        public List<SgnCircularDetail> GetCircularDetails(int? circularId = null)
        {
            return _circularDetails.Where(d => d.AKTIF_PASIF && 
                (circularId == null || d.SGN_CIRCULAR_ID == circularId)).ToList();
        }

        public SgnCircularDetail GetCircularDetailById(int id)
        {
            return _circularDetails.FirstOrDefault(d => d.ID == id && d.AKTIF_PASIF);
        }

        public int InsertCircularDetail(SgnCircularDetail detail)
        {
            detail.ID = _nextDetailId++;
            detail.KAYIT_TARIHI = DateTime.Now;
            detail.AKTIF_PASIF = true;
            _circularDetails.Add(detail);
            return detail.ID;
        }

        public bool UpdateCircularDetail(SgnCircularDetail detail)
        {
            var existing = _circularDetails.FirstOrDefault(d => d.ID == detail.ID);
            if (existing != null)
            {
                existing.SGN_CIRCULAR_ID = detail.SGN_CIRCULAR_ID;
                existing.ADI_SOYADI = detail.ADI_SOYADI;
                existing.YETKI_SEKLI = detail.YETKI_SEKLI;
                existing.YETKI_SURE = detail.YETKI_SURE;
                existing.YETKI_BITIS_TARIHI = detail.YETKI_BITIS_TARIHI;
                existing.IMZA_YETKI_GRUBU = detail.IMZA_YETKI_GRUBU;
                existing.SINIRLI_YETKI_VAR_MI = detail.SINIRLI_YETKI_VAR_MI;
                existing.YETKI_OLDUGU_ISLEMLER = detail.YETKI_OLDUGU_ISLEMLER;
                existing.IMZA_GORUNTUSU = detail.IMZA_GORUNTUSU;
                existing.IMZA_KOORDINAT_X = detail.IMZA_KOORDINAT_X;
                existing.IMZA_KOORDINAT_Y = detail.IMZA_KOORDINAT_Y;
                existing.IMZA_KOORDINAT_WIDTH = detail.IMZA_KOORDINAT_WIDTH;
                existing.IMZA_KOORDINAT_HEIGHT = detail.IMZA_KOORDINAT_HEIGHT;
                return true;
            }
            return false;
        }

        public bool DeleteCircularDetail(int id)
        {
            var detail = _circularDetails.FirstOrDefault(d => d.ID == id);
            if (detail != null)
            {
                detail.AKTIF_PASIF = false;
                return true;
            }
            return false;
        }

        // SGN_OPERATION Methods
        public List<SgnOperation> GetOperations(int? circularId = null)
        {
            return _operations.Where(o => o.AKTIF_PASIF && 
                (circularId == null || o.SGN_CIRCULAR_ID == circularId)).ToList();
        }

        public int InsertOperation(SgnOperation operation)
        {
            operation.ID = _nextOperationId++;
            operation.KAYIT_TARIHI = DateTime.Now;
            operation.AKTIF_PASIF = true;
            _operations.Add(operation);
            return operation.ID;
        }

        // SGN_ROLETYPE Methods
        public List<SgnRoleType> GetRoleTypes(int? circularId = null)
        {
            return _roleTypes.Where(r => r.AKTIF_PASIF && 
                (circularId == null || r.SGN_CIRCULAR_ID == circularId)).ToList();
        }

        public int InsertRoleType(SgnRoleType roleType)
        {
            roleType.ID = _nextRoleTypeId++;
            roleType.KAYIT_TARIHI = DateTime.Now;
            roleType.AKTIF_PASIF = true;
            _roleTypes.Add(roleType);
            return roleType.ID;
        }
    }
} 