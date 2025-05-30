using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using PDFSystem.Models;

namespace PDFSystem.Services
{
    public class SignatureCircularService
    {
        private string connectionString;

        public SignatureCircularService()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString 
                ?? "Data Source=.;Initial Catalog=PDFSignatureSystem;Integrated Security=True";
        }

        #region SignatureCircular Methods

        public List<SignatureCircular> GetSignatureCircularsByContractId(int contractId)
        {
            var circulars = new List<SignatureCircular>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT SignatureCircularId, ContractId, CircularTitle, Description, 
                           PDFFilePath, PDFFileName, PDFContent, CreatedDate, DueDate, Status, IsActive
                    FROM SignatureCirculars 
                    WHERE ContractId = @ContractId AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contractId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            circulars.Add(new SignatureCircular
                            {
                                SignatureCircularId = reader.GetInt32("SignatureCircularId"),
                                ContractId = reader.GetInt32("ContractId"),
                                CircularTitle = reader.GetString("CircularTitle"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                                PDFFilePath = reader.IsDBNull("PDFFilePath") ? null : reader.GetString("PDFFilePath"),
                                PDFFileName = reader.IsDBNull("PDFFileName") ? null : reader.GetString("PDFFileName"),
                                PDFContent = reader.IsDBNull("PDFContent") ? null : (byte[])reader["PDFContent"],
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Status = reader.GetString("Status"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }

            return circulars;
        }

        public SignatureCircular GetSignatureCircularById(int signatureCircularId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT SignatureCircularId, ContractId, CircularTitle, Description, 
                           PDFFilePath, PDFFileName, PDFContent, CreatedDate, DueDate, Status, IsActive
                    FROM SignatureCirculars 
                    WHERE SignatureCircularId = @SignatureCircularId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SignatureCircular
                            {
                                SignatureCircularId = reader.GetInt32("SignatureCircularId"),
                                ContractId = reader.GetInt32("ContractId"),
                                CircularTitle = reader.GetString("CircularTitle"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                                PDFFilePath = reader.IsDBNull("PDFFilePath") ? null : reader.GetString("PDFFilePath"),
                                PDFFileName = reader.IsDBNull("PDFFileName") ? null : reader.GetString("PDFFileName"),
                                PDFContent = reader.IsDBNull("PDFContent") ? null : (byte[])reader["PDFContent"],
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                DueDate = reader.IsDBNull("DueDate") ? (DateTime?)null : reader.GetDateTime("DueDate"),
                                Status = reader.GetString("Status"),
                                IsActive = reader.GetBoolean("IsActive")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int InsertSignatureCircular(SignatureCircular circular)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    INSERT INTO SignatureCirculars (ContractId, CircularTitle, Description, PDFFilePath, PDFFileName, PDFContent, CreatedDate, DueDate, Status, IsActive)
                    VALUES (@ContractId, @CircularTitle, @Description, @PDFFilePath, @PDFFileName, @PDFContent, @CreatedDate, @DueDate, @Status, @IsActive);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", circular.ContractId);
                    command.Parameters.AddWithValue("@CircularTitle", circular.CircularTitle);
                    command.Parameters.AddWithValue("@Description", (object)circular.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PDFFilePath", (object)circular.PDFFilePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PDFFileName", (object)circular.PDFFileName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PDFContent", (object)circular.PDFContent ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", circular.CreatedDate);
                    command.Parameters.AddWithValue("@DueDate", (object)circular.DueDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", circular.Status);
                    command.Parameters.AddWithValue("@IsActive", circular.IsActive);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateSignatureCircular(SignatureCircular circular)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE SignatureCirculars 
                    SET CircularTitle = @CircularTitle, 
                        Description = @Description, 
                        DueDate = @DueDate, 
                        Status = @Status,
                        IsActive = @IsActive
                    WHERE SignatureCircularId = @SignatureCircularId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", circular.SignatureCircularId);
                    command.Parameters.AddWithValue("@CircularTitle", circular.CircularTitle);
                    command.Parameters.AddWithValue("@Description", (object)circular.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", (object)circular.DueDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", circular.Status);
                    command.Parameters.AddWithValue("@IsActive", circular.IsActive);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSignatureCircular(int signatureCircularId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Önce tüm imza atamalarını sil
                        var deleteAssignmentsQuery = "UPDATE SignatureAssignments SET IsActive = 0 WHERE SignatureCircularId = @SignatureCircularId";
                        using (var command = new SqlCommand(deleteAssignmentsQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);
                            command.ExecuteNonQuery();
                        }

                        // Sonra sirküler'ı sil
                        var deleteCircularQuery = "UPDATE SignatureCirculars SET IsActive = 0 WHERE SignatureCircularId = @SignatureCircularId";
                        using (var command = new SqlCommand(deleteCircularQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public int GetSignatureAssignmentCount(int signatureCircularId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM SignatureAssignments WHERE SignatureCircularId = @SignatureCircularId AND IsActive = 1";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        #endregion

        #region SignatureAssignment Methods

        public List<SignatureAssignment> GetSignatureAssignmentsByCircularId(int signatureCircularId)
        {
            var assignments = new List<SignatureAssignment>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT SignatureAssignmentId, SignatureCircularId, AssignedPersonName, AssignedPersonEmail, 
                           AssignedPersonTitle, AssignedPersonDepartment, PDFPageNumber, SignatureX, SignatureY, 
                           SignatureWidth, SignatureHeight, SignatureImagePath, SignatureImageData, SignatureImageFormat,
                           AssignedDate, SignedDate, Status, Notes, IsActive
                    FROM SignatureAssignments 
                    WHERE SignatureCircularId = @SignatureCircularId AND IsActive = 1
                    ORDER BY AssignedDate";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            assignments.Add(new SignatureAssignment
                            {
                                SignatureAssignmentId = reader.GetInt32("SignatureAssignmentId"),
                                SignatureCircularId = reader.GetInt32("SignatureCircularId"),
                                AssignedPersonName = reader.GetString("AssignedPersonName"),
                                AssignedPersonEmail = reader.IsDBNull("AssignedPersonEmail") ? null : reader.GetString("AssignedPersonEmail"),
                                AssignedPersonTitle = reader.IsDBNull("AssignedPersonTitle") ? null : reader.GetString("AssignedPersonTitle"),
                                AssignedPersonDepartment = reader.IsDBNull("AssignedPersonDepartment") ? null : reader.GetString("AssignedPersonDepartment"),
                                PDFPageNumber = reader.GetInt32("PDFPageNumber"),
                                SignatureX = reader.GetFloat("SignatureX"),
                                SignatureY = reader.GetFloat("SignatureY"),
                                SignatureWidth = reader.GetFloat("SignatureWidth"),
                                SignatureHeight = reader.GetFloat("SignatureHeight"),
                                SignatureImagePath = reader.IsDBNull("SignatureImagePath") ? null : reader.GetString("SignatureImagePath"),
                                SignatureImageData = reader.IsDBNull("SignatureImageData") ? null : (byte[])reader["SignatureImageData"],
                                SignatureImageFormat = reader.IsDBNull("SignatureImageFormat") ? null : reader.GetString("SignatureImageFormat"),
                                AssignedDate = reader.GetDateTime("AssignedDate"),
                                SignedDate = reader.IsDBNull("SignedDate") ? (DateTime?)null : reader.GetDateTime("SignedDate"),
                                Status = reader.GetString("Status"),
                                Notes = reader.IsDBNull("Notes") ? null : reader.GetString("Notes"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }

            return assignments;
        }

        public int InsertSignatureAssignment(SignatureAssignment assignment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    INSERT INTO SignatureAssignments (SignatureCircularId, AssignedPersonName, AssignedPersonEmail, AssignedPersonTitle, 
                                                     AssignedPersonDepartment, PDFPageNumber, SignatureX, SignatureY, SignatureWidth, SignatureHeight,
                                                     SignatureImagePath, SignatureImageData, SignatureImageFormat, AssignedDate, SignedDate, 
                                                     Status, Notes, IsActive)
                    VALUES (@SignatureCircularId, @AssignedPersonName, @AssignedPersonEmail, @AssignedPersonTitle, 
                            @AssignedPersonDepartment, @PDFPageNumber, @SignatureX, @SignatureY, @SignatureWidth, @SignatureHeight,
                            @SignatureImagePath, @SignatureImageData, @SignatureImageFormat, @AssignedDate, @SignedDate, 
                            @Status, @Notes, @IsActive);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", assignment.SignatureCircularId);
                    command.Parameters.AddWithValue("@AssignedPersonName", assignment.AssignedPersonName);
                    command.Parameters.AddWithValue("@AssignedPersonEmail", (object)assignment.AssignedPersonEmail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedPersonTitle", (object)assignment.AssignedPersonTitle ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedPersonDepartment", (object)assignment.AssignedPersonDepartment ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PDFPageNumber", assignment.PDFPageNumber);
                    command.Parameters.AddWithValue("@SignatureX", assignment.SignatureX);
                    command.Parameters.AddWithValue("@SignatureY", assignment.SignatureY);
                    command.Parameters.AddWithValue("@SignatureWidth", assignment.SignatureWidth);
                    command.Parameters.AddWithValue("@SignatureHeight", assignment.SignatureHeight);
                    command.Parameters.AddWithValue("@SignatureImagePath", (object)assignment.SignatureImagePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignatureImageData", (object)assignment.SignatureImageData ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignatureImageFormat", (object)assignment.SignatureImageFormat ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedDate", assignment.AssignedDate);
                    command.Parameters.AddWithValue("@SignedDate", (object)assignment.SignedDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", assignment.Status);
                    command.Parameters.AddWithValue("@Notes", (object)assignment.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", assignment.IsActive);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateSignatureAssignment(SignatureAssignment assignment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE SignatureAssignments 
                    SET AssignedPersonName = @AssignedPersonName,
                        AssignedPersonEmail = @AssignedPersonEmail,
                        AssignedPersonTitle = @AssignedPersonTitle,
                        AssignedPersonDepartment = @AssignedPersonDepartment,
                        SignatureImagePath = @SignatureImagePath,
                        SignatureImageData = @SignatureImageData,
                        SignatureImageFormat = @SignatureImageFormat,
                        SignedDate = @SignedDate,
                        Status = @Status,
                        Notes = @Notes,
                        IsActive = @IsActive
                    WHERE SignatureAssignmentId = @SignatureAssignmentId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureAssignmentId", assignment.SignatureAssignmentId);
                    command.Parameters.AddWithValue("@AssignedPersonName", assignment.AssignedPersonName);
                    command.Parameters.AddWithValue("@AssignedPersonEmail", (object)assignment.AssignedPersonEmail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedPersonTitle", (object)assignment.AssignedPersonTitle ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AssignedPersonDepartment", (object)assignment.AssignedPersonDepartment ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignatureImagePath", (object)assignment.SignatureImagePath ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignatureImageData", (object)assignment.SignatureImageData ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignatureImageFormat", (object)assignment.SignatureImageFormat ?? DBNull.Value);
                    command.Parameters.AddWithValue("@SignedDate", (object)assignment.SignedDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", assignment.Status);
                    command.Parameters.AddWithValue("@Notes", (object)assignment.Notes ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", assignment.IsActive);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveSignatureAssignments(List<SignatureAssignment> assignments)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var assignment in assignments)
                        {
                            if (assignment.SignatureAssignmentId == 0)
                            {
                                // Yeni kayıt
                                assignment.SignatureAssignmentId = InsertSignatureAssignment(assignment);
                            }
                            else
                            {
                                // Güncelleme
                                UpdateSignatureAssignment(assignment);
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public byte[] GetPDFContent(int signatureCircularId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT PDFContent FROM SignatureCirculars WHERE SignatureCircularId = @SignatureCircularId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);
                    var result = command.ExecuteScalar();
                    return result as byte[];
                }
            }
        }

        public void SavePDFWithSignatures(int signatureCircularId, byte[] signedPDFContent)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE SignatureCirculars 
                    SET PDFContent = @PDFContent, Status = 'Completed'
                    WHERE SignatureCircularId = @SignatureCircularId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SignatureCircularId", signatureCircularId);
                    command.Parameters.AddWithValue("@PDFContent", signedPDFContent);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
} 