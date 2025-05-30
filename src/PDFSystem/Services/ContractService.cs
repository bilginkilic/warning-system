using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PDFSystem.Models;

namespace PDFSystem.Services
{
    public class ContractService
    {
        private string connectionString;

        public ContractService()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString 
                ?? "Data Source=.;Initial Catalog=PDFSignatureSystem;Integrated Security=True";
        }

        public List<Contract> GetContractsByCustomerId(int customerId)
        {
            var contracts = new List<Contract>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT c.ContractId, c.CustomerId, c.ContractNumber, c.ContractTitle, c.Description, 
                           c.CreatedDate, c.StartDate, c.EndDate, c.ContractAmount, c.Status, c.IsActive,
                           cust.CustomerName
                    FROM Contracts c
                    INNER JOIN Customers cust ON c.CustomerId = cust.CustomerId
                    WHERE c.CustomerId = @CustomerId AND c.IsActive = 1
                    ORDER BY c.CreatedDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contracts.Add(new Contract
                            {
                                ContractId = reader.GetInt32("ContractId"),
                                CustomerId = reader.GetInt32("CustomerId"),
                                ContractNumber = reader.GetString("ContractNumber"),
                                ContractTitle = reader.GetString("ContractTitle"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                StartDate = reader.IsDBNull("StartDate") ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                EndDate = reader.IsDBNull("EndDate") ? (DateTime?)null : reader.GetDateTime("EndDate"),
                                ContractAmount = reader.IsDBNull("ContractAmount") ? (decimal?)null : reader.GetDecimal("ContractAmount"),
                                Status = reader.GetString("Status"),
                                IsActive = reader.GetBoolean("IsActive"),
                                Customer = new Customer
                                {
                                    CustomerId = reader.GetInt32("CustomerId"),
                                    CustomerName = reader.GetString("CustomerName")
                                }
                            });
                        }
                    }
                }
            }

            return contracts;
        }

        public Contract GetContractById(int contractId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT c.ContractId, c.CustomerId, c.ContractNumber, c.ContractTitle, c.Description, 
                           c.CreatedDate, c.StartDate, c.EndDate, c.ContractAmount, c.Status, c.IsActive,
                           cust.CustomerName, cust.CustomerCode, cust.Email, cust.Phone, cust.Address
                    FROM Contracts c
                    INNER JOIN Customers cust ON c.CustomerId = cust.CustomerId
                    WHERE c.ContractId = @ContractId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contractId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Contract
                            {
                                ContractId = reader.GetInt32("ContractId"),
                                CustomerId = reader.GetInt32("CustomerId"),
                                ContractNumber = reader.GetString("ContractNumber"),
                                ContractTitle = reader.GetString("ContractTitle"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                StartDate = reader.IsDBNull("StartDate") ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                EndDate = reader.IsDBNull("EndDate") ? (DateTime?)null : reader.GetDateTime("EndDate"),
                                ContractAmount = reader.IsDBNull("ContractAmount") ? (decimal?)null : reader.GetDecimal("ContractAmount"),
                                Status = reader.GetString("Status"),
                                IsActive = reader.GetBoolean("IsActive"),
                                Customer = new Customer
                                {
                                    CustomerId = reader.GetInt32("CustomerId"),
                                    CustomerName = reader.GetString("CustomerName"),
                                    CustomerCode = reader.IsDBNull("CustomerCode") ? null : reader.GetString("CustomerCode"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                                    Address = reader.IsDBNull("Address") ? null : reader.GetString("Address")
                                }
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int InsertContract(Contract contract)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    INSERT INTO Contracts (CustomerId, ContractNumber, ContractTitle, Description, CreatedDate, StartDate, EndDate, ContractAmount, Status, IsActive)
                    VALUES (@CustomerId, @ContractNumber, @ContractTitle, @Description, @CreatedDate, @StartDate, @EndDate, @ContractAmount, @Status, @IsActive);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", contract.CustomerId);
                    command.Parameters.AddWithValue("@ContractNumber", contract.ContractNumber);
                    command.Parameters.AddWithValue("@ContractTitle", contract.ContractTitle);
                    command.Parameters.AddWithValue("@Description", (object)contract.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", contract.CreatedDate);
                    command.Parameters.AddWithValue("@StartDate", (object)contract.StartDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", (object)contract.EndDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContractAmount", (object)contract.ContractAmount ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", contract.Status);
                    command.Parameters.AddWithValue("@IsActive", contract.IsActive);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateContract(Contract contract)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE Contracts 
                    SET ContractNumber = @ContractNumber, 
                        ContractTitle = @ContractTitle, 
                        Description = @Description, 
                        StartDate = @StartDate, 
                        EndDate = @EndDate, 
                        ContractAmount = @ContractAmount, 
                        Status = @Status,
                        IsActive = @IsActive
                    WHERE ContractId = @ContractId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contract.ContractId);
                    command.Parameters.AddWithValue("@ContractNumber", contract.ContractNumber);
                    command.Parameters.AddWithValue("@ContractTitle", contract.ContractTitle);
                    command.Parameters.AddWithValue("@Description", (object)contract.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StartDate", (object)contract.StartDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", (object)contract.EndDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContractAmount", (object)contract.ContractAmount ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", contract.Status);
                    command.Parameters.AddWithValue("@IsActive", contract.IsActive);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteContract(int contractId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Contracts SET IsActive = 0 WHERE ContractId = @ContractId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contractId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public int GetSignatureCircularCount(int contractId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM SignatureCirculars WHERE ContractId = @ContractId AND IsActive = 1";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contractId);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<Contract> SearchContracts(string searchTerm)
        {
            var contracts = new List<Contract>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT c.ContractId, c.CustomerId, c.ContractNumber, c.ContractTitle, c.Description, 
                           c.CreatedDate, c.StartDate, c.EndDate, c.ContractAmount, c.Status, c.IsActive,
                           cust.CustomerName
                    FROM Contracts c
                    INNER JOIN Customers cust ON c.CustomerId = cust.CustomerId
                    WHERE c.IsActive = 1 
                    AND (c.ContractNumber LIKE @SearchTerm 
                         OR c.ContractTitle LIKE @SearchTerm 
                         OR c.Description LIKE @SearchTerm
                         OR cust.CustomerName LIKE @SearchTerm)
                    ORDER BY c.CreatedDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contracts.Add(new Contract
                            {
                                ContractId = reader.GetInt32("ContractId"),
                                CustomerId = reader.GetInt32("CustomerId"),
                                ContractNumber = reader.GetString("ContractNumber"),
                                ContractTitle = reader.GetString("ContractTitle"),
                                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                StartDate = reader.IsDBNull("StartDate") ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                EndDate = reader.IsDBNull("EndDate") ? (DateTime?)null : reader.GetDateTime("EndDate"),
                                ContractAmount = reader.IsDBNull("ContractAmount") ? (decimal?)null : reader.GetDecimal("ContractAmount"),
                                Status = reader.GetString("Status"),
                                IsActive = reader.GetBoolean("IsActive"),
                                Customer = new Customer
                                {
                                    CustomerId = reader.GetInt32("CustomerId"),
                                    CustomerName = reader.GetString("CustomerName")
                                }
                            });
                        }
                    }
                }
            }

            return contracts;
        }

        public string GenerateContractNumber()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT COUNT(*) 
                    FROM Contracts 
                    WHERE YEAR(CreatedDate) = @Year AND MONTH(CreatedDate) = @Month";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month);
                    
                    var count = (int)command.ExecuteScalar();
                    return $"CONT-{year}{month:D2}-{(count + 1):D4}";
                }
            }
        }
    }
} 