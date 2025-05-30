using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PDFSystem.Models;

namespace PDFSystem.Services
{
    public class CustomerService
    {
        private string connectionString;

        public CustomerService()
        {
            // Connection string'i config'den okuyun
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString 
                ?? "Data Source=.;Initial Catalog=PDFSignatureSystem;Integrated Security=True";
        }

        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT CustomerId, CustomerName, CustomerCode, Email, Phone, Address, CreatedDate, IsActive 
                    FROM Customers 
                    WHERE IsActive = 1 
                    ORDER BY CustomerName";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32("CustomerId"),
                                CustomerName = reader.GetString("CustomerName"),
                                CustomerCode = reader.IsDBNull("CustomerCode") ? null : reader.GetString("CustomerCode"),
                                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                                Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }

            return customers;
        }

        public Customer GetCustomerById(int customerId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT CustomerId, CustomerName, CustomerCode, Email, Phone, Address, CreatedDate, IsActive 
                    FROM Customers 
                    WHERE CustomerId = @CustomerId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                CustomerId = reader.GetInt32("CustomerId"),
                                CustomerName = reader.GetString("CustomerName"),
                                CustomerCode = reader.IsDBNull("CustomerCode") ? null : reader.GetString("CustomerCode"),
                                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                                Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                IsActive = reader.GetBoolean("IsActive")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int InsertCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    INSERT INTO Customers (CustomerName, CustomerCode, Email, Phone, Address, CreatedDate, IsActive)
                    VALUES (@CustomerName, @CustomerCode, @Email, @Phone, @Address, @CreatedDate, @IsActive);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    command.Parameters.AddWithValue("@CustomerCode", (object)customer.CustomerCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", customer.CreatedDate);
                    command.Parameters.AddWithValue("@IsActive", customer.IsActive);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE Customers 
                    SET CustomerName = @CustomerName, 
                        CustomerCode = @CustomerCode, 
                        Email = @Email, 
                        Phone = @Phone, 
                        Address = @Address, 
                        IsActive = @IsActive
                    WHERE CustomerId = @CustomerId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    command.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    command.Parameters.AddWithValue("@CustomerCode", (object)customer.CustomerCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", customer.IsActive);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCustomer(int customerId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Customers SET IsActive = 0 WHERE CustomerId = @CustomerId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Customer> SearchCustomers(string searchTerm)
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT CustomerId, CustomerName, CustomerCode, Email, Phone, Address, CreatedDate, IsActive 
                    FROM Customers 
                    WHERE IsActive = 1 
                    AND (CustomerName LIKE @SearchTerm 
                         OR CustomerCode LIKE @SearchTerm 
                         OR Email LIKE @SearchTerm)
                    ORDER BY CustomerName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32("CustomerId"),
                                CustomerName = reader.GetString("CustomerName"),
                                CustomerCode = reader.IsDBNull("CustomerCode") ? null : reader.GetString("CustomerCode"),
                                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                                Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                IsActive = reader.GetBoolean("IsActive")
                            });
                        }
                    }
                }
            }

            return customers;
        }
    }
} 