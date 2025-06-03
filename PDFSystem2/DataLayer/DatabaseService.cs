using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PDFSystem2.Models;

namespace PDFSystem2.DataLayer
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region SGN_CIRCULAR Methods

        public int InsertCircular(SgnCircular circular)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULAR_INS_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FIRMA_UNVANI", circular.FIRMA_UNVANI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FIRMA_HESAP_NUMARASI", circular.FIRMA_HESAP_NUMARASI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_DUZENLEME_TARIHI", circular.IMZA_SIRKULERI_DUZENLEME_TARIHI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_GECERLILIK_TARIHI", circular.IMZA_SIRKULERI_GECERLILIK_TARIHI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SURESIZ_GECERLI", circular.SURESIZ_GECERLI);
                    command.Parameters.AddWithValue("@OZEL_DURUMLAR", circular.OZEL_DURUMLAR ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NOTER_IMZA_SIRKULERI_NO", circular.NOTER_IMZA_SIRKULERI_NO ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_GORUNTUSU", circular.IMZA_SIRKULERI_GORUNTUSU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ACIKLAMA", circular.ACIKLAMA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@KULLANICI", circular.KULLANICI ?? (object)DBNull.Value);

                    var outputParam = new SqlParameter("@NEW_ID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputParam.Value;
                }
            }
        }

        public bool UpdateCircular(SgnCircular circular)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULAR_UPD_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ID", circular.ID);
                    command.Parameters.AddWithValue("@FIRMA_UNVANI", circular.FIRMA_UNVANI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FIRMA_HESAP_NUMARASI", circular.FIRMA_HESAP_NUMARASI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_DUZENLEME_TARIHI", circular.IMZA_SIRKULERI_DUZENLEME_TARIHI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_GECERLILIK_TARIHI", circular.IMZA_SIRKULERI_GECERLILIK_TARIHI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SURESIZ_GECERLI", circular.SURESIZ_GECERLI);
                    command.Parameters.AddWithValue("@OZEL_DURUMLAR", circular.OZEL_DURUMLAR ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NOTER_IMZA_SIRKULERI_NO", circular.NOTER_IMZA_SIRKULERI_NO ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_SIRKULERI_GORUNTUSU", circular.IMZA_SIRKULERI_GORUNTUSU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AKTIF_PASIF", circular.AKTIF_PASIF);
                    command.Parameters.AddWithValue("@ACIKLAMA", circular.ACIKLAMA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@KULLANICI", circular.KULLANICI ?? (object)DBNull.Value);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public List<SgnCircular> GetCirculars(int? id = null)
        {
            var circulars = new List<SgnCircular>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULAR_SEL_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id ?? (object)DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            circulars.Add(new SgnCircular
                            {
                                ID = reader.GetInt32("ID"),
                                FIRMA_UNVANI = reader.IsDBNull("FIRMA_UNVANI") ? null : reader.GetString("FIRMA_UNVANI"),
                                FIRMA_HESAP_NUMARASI = reader.IsDBNull("FIRMA_HESAP_NUMARASI") ? null : reader.GetString("FIRMA_HESAP_NUMARASI"),
                                IMZA_SIRKULERI_DUZENLEME_TARIHI = reader.IsDBNull("IMZA_SIRKULERI_DUZENLEME_TARIHI") ? null : (DateTime?)reader.GetDateTime("IMZA_SIRKULERI_DUZENLEME_TARIHI"),
                                IMZA_SIRKULERI_GECERLILIK_TARIHI = reader.IsDBNull("IMZA_SIRKULERI_GECERLILIK_TARIHI") ? null : (DateTime?)reader.GetDateTime("IMZA_SIRKULERI_GECERLILIK_TARIHI"),
                                SURESIZ_GECERLI = reader.GetBoolean("SURESIZ_GECERLI"),
                                OZEL_DURUMLAR = reader.IsDBNull("OZEL_DURUMLAR") ? null : reader.GetString("OZEL_DURUMLAR"),
                                NOTER_IMZA_SIRKULERI_NO = reader.IsDBNull("NOTER_IMZA_SIRKULERI_NO") ? null : reader.GetString("NOTER_IMZA_SIRKULERI_NO"),
                                IMZA_SIRKULERI_GORUNTUSU = reader.IsDBNull("IMZA_SIRKULERI_GORUNTUSU") ? null : reader.GetString("IMZA_SIRKULERI_GORUNTUSU"),
                                AKTIF_PASIF = reader.GetBoolean("AKTIF_PASIF"),
                                ACIKLAMA = reader.IsDBNull("ACIKLAMA") ? null : reader.GetString("ACIKLAMA"),
                                KULLANICI = reader.IsDBNull("KULLANICI") ? null : reader.GetString("KULLANICI"),
                                KAYIT_TARIHI = reader.GetDateTime("KAYIT_TARIHI"),
                                GUNCELLEME_TARIHI = reader.IsDBNull("GUNCELLEME_TARIHI") ? null : (DateTime?)reader.GetDateTime("GUNCELLEME_TARIHI")
                            });
                        }
                    }
                }
            }

            return circulars;
        }

        public bool DeleteCircular(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULAR_DEL_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        #endregion

        #region SGN_CIRCULARDETAIL Methods

        public int InsertCircularDetail(SgnCircularDetail detail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULARDETAIL_INS_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", detail.SGN_CIRCULAR_ID);
                    command.Parameters.AddWithValue("@ADI_SOYADI", detail.ADI_SOYADI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@YETKI_SEKLI", detail.YETKI_SEKLI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@YETKI_SURE", detail.YETKI_SURE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@YETKI_BITIS_TARIHI", detail.YETKI_BITIS_TARIHI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_YETKI_GRUBU", detail.IMZA_YETKI_GRUBU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SINIRLI_YETKI_VAR_MI", detail.SINIRLI_YETKI_VAR_MI);
                    command.Parameters.AddWithValue("@YETKI_OLDUGU_ISLEMLER", detail.YETKI_OLDUGU_ISLEMLER ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_GORUNTUSU", detail.IMZA_GORUNTUSU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_KOORDINAT_X", detail.IMZA_KOORDINAT_X ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_KOORDINAT_Y", detail.IMZA_KOORDINAT_Y ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_KOORDINAT_WIDTH", detail.IMZA_KOORDINAT_WIDTH ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IMZA_KOORDINAT_HEIGHT", detail.IMZA_KOORDINAT_HEIGHT ?? (object)DBNull.Value);

                    var outputParam = new SqlParameter("@NEW_ID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputParam.Value;
                }
            }
        }

        public List<SgnCircularDetail> GetCircularDetails(int? id = null, int? circularId = null)
        {
            var details = new List<SgnCircularDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_CIRCULARDETAIL_SEL_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", circularId ?? (object)DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(new SgnCircularDetail
                            {
                                ID = reader.GetInt32("ID"),
                                SGN_CIRCULAR_ID = reader.GetInt32("SGN_CIRCULAR_ID"),
                                ADI_SOYADI = reader.IsDBNull("ADI_SOYADI") ? null : reader.GetString("ADI_SOYADI"),
                                YETKI_SEKLI = reader.IsDBNull("YETKI_SEKLI") ? null : reader.GetString("YETKI_SEKLI"),
                                YETKI_SURE = reader.IsDBNull("YETKI_SURE") ? null : reader.GetString("YETKI_SURE"),
                                YETKI_BITIS_TARIHI = reader.IsDBNull("YETKI_BITIS_TARIHI") ? null : (DateTime?)reader.GetDateTime("YETKI_BITIS_TARIHI"),
                                IMZA_YETKI_GRUBU = reader.IsDBNull("IMZA_YETKI_GRUBU") ? null : reader.GetString("IMZA_YETKI_GRUBU"),
                                SINIRLI_YETKI_VAR_MI = reader.GetBoolean("SINIRLI_YETKI_VAR_MI"),
                                YETKI_OLDUGU_ISLEMLER = reader.IsDBNull("YETKI_OLDUGU_ISLEMLER") ? null : reader.GetString("YETKI_OLDUGU_ISLEMLER"),
                                IMZA_GORUNTUSU = reader.IsDBNull("IMZA_GORUNTUSU") ? null : reader.GetString("IMZA_GORUNTUSU"),
                                IMZA_KOORDINAT_X = reader.IsDBNull("IMZA_KOORDINAT_X") ? null : (int?)reader.GetInt32("IMZA_KOORDINAT_X"),
                                IMZA_KOORDINAT_Y = reader.IsDBNull("IMZA_KOORDINAT_Y") ? null : (int?)reader.GetInt32("IMZA_KOORDINAT_Y"),
                                IMZA_KOORDINAT_WIDTH = reader.IsDBNull("IMZA_KOORDINAT_WIDTH") ? null : (int?)reader.GetInt32("IMZA_KOORDINAT_WIDTH"),
                                IMZA_KOORDINAT_HEIGHT = reader.IsDBNull("IMZA_KOORDINAT_HEIGHT") ? null : (int?)reader.GetInt32("IMZA_KOORDINAT_HEIGHT"),
                                AKTIF_PASIF = reader.GetBoolean("AKTIF_PASIF"),
                                KAYIT_TARIHI = reader.GetDateTime("KAYIT_TARIHI")
                            });
                        }
                    }
                }
            }

            return details;
        }

        #endregion

        #region SGN_OPERATION Methods

        public int InsertOperation(SgnOperation operation)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_OPERATION_INS_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", operation.SGN_CIRCULAR_ID);
                    command.Parameters.AddWithValue("@OPERATION_TYPE", operation.OPERATION_TYPE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OPERATION_CODE", operation.OPERATION_CODE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ACIKLAMA", operation.ACIKLAMA ?? (object)DBNull.Value);

                    var outputParam = new SqlParameter("@NEW_ID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputParam.Value;
                }
            }
        }

        public List<SgnOperation> GetOperations(int? id = null, int? circularId = null)
        {
            var operations = new List<SgnOperation>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_OPERATION_SEL_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", circularId ?? (object)DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            operations.Add(new SgnOperation
                            {
                                ID = reader.GetInt32("ID"),
                                SGN_CIRCULAR_ID = reader.GetInt32("SGN_CIRCULAR_ID"),
                                OPERATION_TYPE = reader.IsDBNull("OPERATION_TYPE") ? null : reader.GetString("OPERATION_TYPE"),
                                OPERATION_CODE = reader.IsDBNull("OPERATION_CODE") ? null : reader.GetString("OPERATION_CODE"),
                                ACIKLAMA = reader.IsDBNull("ACIKLAMA") ? null : reader.GetString("ACIKLAMA"),
                                AKTIF_PASIF = reader.GetBoolean("AKTIF_PASIF"),
                                KAYIT_TARIHI = reader.GetDateTime("KAYIT_TARIHI")
                            });
                        }
                    }
                }
            }

            return operations;
        }

        #endregion

        #region SGN_ROLETYPE Methods

        public int InsertRoleType(SgnRoleType roleType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_ROLETYPE_INS_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", roleType.SGN_CIRCULAR_ID);
                    command.Parameters.AddWithValue("@ROLE_GROUP", roleType.ROLE_GROUP ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ROLE_TYPE", roleType.ROLE_TYPE ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MIN_SIGNATURE_COUNT", roleType.MIN_SIGNATURE_COUNT);
                    command.Parameters.AddWithValue("@MAX_SIGNATURE_COUNT", roleType.MAX_SIGNATURE_COUNT ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@YETKI_LIMITI", roleType.YETKI_LIMITI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PARA_BIRIMI", roleType.PARA_BIRIMI ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ACIKLAMA", roleType.ACIKLAMA ?? (object)DBNull.Value);

                    var outputParam = new SqlParameter("@NEW_ID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputParam.Value;
                }
            }
        }

        public List<SgnRoleType> GetRoleTypes(int? id = null, int? circularId = null)
        {
            var roleTypes = new List<SgnRoleType>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SGN_ROLETYPE_SEL_SP", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SGN_CIRCULAR_ID", circularId ?? (object)DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roleTypes.Add(new SgnRoleType
                            {
                                ID = reader.GetInt32("ID"),
                                SGN_CIRCULAR_ID = reader.GetInt32("SGN_CIRCULAR_ID"),
                                ROLE_GROUP = reader.IsDBNull("ROLE_GROUP") ? null : reader.GetString("ROLE_GROUP"),
                                ROLE_TYPE = reader.IsDBNull("ROLE_TYPE") ? null : reader.GetString("ROLE_TYPE"),
                                MIN_SIGNATURE_COUNT = reader.GetInt32("MIN_SIGNATURE_COUNT"),
                                MAX_SIGNATURE_COUNT = reader.IsDBNull("MAX_SIGNATURE_COUNT") ? null : (int?)reader.GetInt32("MAX_SIGNATURE_COUNT"),
                                YETKI_LIMITI = reader.IsDBNull("YETKI_LIMITI") ? null : (decimal?)reader.GetDecimal("YETKI_LIMITI"),
                                PARA_BIRIMI = reader.IsDBNull("PARA_BIRIMI") ? null : reader.GetString("PARA_BIRIMI"),
                                ACIKLAMA = reader.IsDBNull("ACIKLAMA") ? null : reader.GetString("ACIKLAMA"),
                                AKTIF_PASIF = reader.GetBoolean("AKTIF_PASIF"),
                                KAYIT_TARIHI = reader.GetDateTime("KAYIT_TARIHI")
                            });
                        }
                    }
                }
            }

            return roleTypes;
        }

        #endregion
    }
} 