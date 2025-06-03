using System;
using System.Collections.Generic;
using System.Linq;
using PDFSystem.Models;

namespace PDFSystem.Services
{
    public class MockDataService
    {
        private static List<Customer> _customers;
        private static List<Contract> _contracts;
        private static List<SignatureCircular> _circulars;
        private static List<SignatureAssignment> _assignments;
        private static List<User> _users;

        static MockDataService()
        {
            InitializeMockData();
        }

        private static void InitializeMockData()
        {
            // Mock Müşteriler
            _customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "ABC Teknoloji A.Ş.", Status = "Aktif", Address = "İstanbul, Maslak", Phone = "0212 555 1111" },
                new Customer { CustomerId = 2, CustomerName = "XYZ Yazılım Ltd. Şti.", Status = "Aktif", Address = "Ankara, Çankaya", Phone = "0312 444 2222" },
                new Customer { CustomerId = 3, CustomerName = "123 Bilişim A.Ş.", Status = "Pasif", Address = "İzmir, Konak", Phone = "0232 333 3333" },
                new Customer { CustomerId = 4, CustomerName = "Tech Solutions Inc.", Status = "Aktif", Address = "İstanbul, Levent", Phone = "0212 666 4444" },
                new Customer { CustomerId = 5, CustomerName = "Dijital Çözümler Ltd.", Status = "Aktif", Address = "Bursa, Nilüfer", Phone = "0224 777 5555" }
            };

            // Mock Kullanıcılar
            _users = new List<User>
            {
                new User { UserId = 1, UserName = "Ahmet Yılmaz", Email = "ahmet@sirket.com", Role = "Yönetici", Status = "Aktif" },
                new User { UserId = 2, UserName = "Mehmet Demir", Email = "mehmet@sirket.com", Role = "İmzacı", Status = "Aktif" },
                new User { UserId = 3, UserName = "Ayşe Kaya", Email = "ayse@sirket.com", Role = "İmzacı", Status = "Aktif" },
                new User { UserId = 4, UserName = "Fatma Şahin", Email = "fatma@sirket.com", Role = "İmzacı", Status = "Aktif" },
                new User { UserId = 5, UserName = "Ali Öztürk", Email = "ali@sirket.com", Role = "Yönetici", Status = "Aktif" }
            };

            // Mock Kontratlar
            _contracts = new List<Contract>
            {
                new Contract { 
                    ContractId = 1, 
                    ContractNo = "CNT-2024-001",
                    CustomerId = 1,
                    ContractDate = DateTime.Now.AddDays(-30),
                    ContractType = "Satış Sözleşmesi",
                    Description = "Yazılım lisans satışı",
                    Status = "Aktif",
                    PDFPath = "C:\\Temp\\Contracts\\contract1.pdf"
                },
                new Contract {
                    ContractId = 2,
                    ContractNo = "CNT-2024-002",
                    CustomerId = 2,
                    ContractDate = DateTime.Now.AddDays(-20),
                    ContractType = "Hizmet Sözleşmesi",
                    Description = "Yıllık bakım hizmeti",
                    Status = "Aktif",
                    PDFPath = "C:\\Temp\\Contracts\\contract2.pdf"
                },
                new Contract {
                    ContractId = 3,
                    ContractNo = "CNT-2024-003",
                    CustomerId = 1,
                    ContractDate = DateTime.Now.AddDays(-10),
                    ContractType = "Bakım Sözleşmesi",
                    Description = "Donanım bakım hizmeti",
                    Status = "Beklemede",
                    PDFPath = "C:\\Temp\\Contracts\\contract3.pdf"
                }
            };

            // Mock İmza Sirkülerleri
            _circulars = new List<SignatureCircular>
            {
                new SignatureCircular {
                    CircularId = 1,
                    ContractId = 1,
                    CreationDate = DateTime.Now.AddDays(-5),
                    Status = "Beklemede",
                    Description = "Yazılım lisans sözleşmesi imza sirküleri"
                },
                new SignatureCircular {
                    CircularId = 2,
                    ContractId = 2,
                    CreationDate = DateTime.Now.AddDays(-3),
                    Status = "İmzada",
                    Description = "Yıllık bakım sözleşmesi imza sirküleri"
                }
            };

            // Mock İmza Atamaları
            _assignments = new List<SignatureAssignment>
            {
                new SignatureAssignment {
                    AssignmentId = 1,
                    CircularId = 1,
                    UserId = 2,
                    Position = "İmzalayacak",
                    Coordinates = "X:100, Y:200, W:150, H:50",
                    Status = "Bekliyor"
                },
                new SignatureAssignment {
                    AssignmentId = 2,
                    CircularId = 1,
                    UserId = 3,
                    Position = "İmzalayacak",
                    Coordinates = "X:100, Y:300, W:150, H:50",
                    Status = "Bekliyor"
                },
                new SignatureAssignment {
                    AssignmentId = 3,
                    CircularId = 2,
                    UserId = 4,
                    Position = "İmzalayacak",
                    Coordinates = "X:100, Y:400, W:150, H:50",
                    Status = "İmzalandı"
                }
            };
        }

        // Mock Data Getter Methods
        public static List<Customer> GetCustomers(string status = null)
        {
            return status == null ? 
                _customers : 
                _customers.Where(c => c.Status == status).ToList();
        }

        public static Customer GetCustomerById(int customerId)
        {
            return _customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public static List<Contract> GetContracts(int? customerId = null)
        {
            return customerId.HasValue ? 
                _contracts.Where(c => c.CustomerId == customerId).ToList() : 
                _contracts;
        }

        public static Contract GetContractById(int contractId)
        {
            return _contracts.FirstOrDefault(c => c.ContractId == contractId);
        }

        public static List<User> GetUsers(string status = null)
        {
            return status == null ? 
                _users : 
                _users.Where(u => u.Status == status).ToList();
        }

        public static User GetUserById(int userId)
        {
            return _users.FirstOrDefault(u => u.UserId == userId);
        }

        public static List<SignatureCircular> GetCirculars(int? contractId = null)
        {
            return contractId.HasValue ? 
                _circulars.Where(s => s.ContractId == contractId).ToList() : 
                _circulars;
        }

        public static SignatureCircular GetCircularById(int circularId)
        {
            return _circulars.FirstOrDefault(s => s.CircularId == circularId);
        }

        public static List<SignatureAssignment> GetAssignments(int? circularId = null)
        {
            return circularId.HasValue ? 
                _assignments.Where(a => a.CircularId == circularId).ToList() : 
                _assignments;
        }

        public static SignatureAssignment GetAssignmentById(int assignmentId)
        {
            return _assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
        }

        // Mock Data Manipulation Methods
        public static void AddCustomer(Customer customer)
        {
            customer.CustomerId = _customers.Max(c => c.CustomerId) + 1;
            _customers.Add(customer);
        }

        public static void AddContract(Contract contract)
        {
            contract.ContractId = _contracts.Max(c => c.ContractId) + 1;
            _contracts.Add(contract);
        }

        public static void AddCircular(SignatureCircular circular)
        {
            circular.CircularId = _circulars.Max(s => s.CircularId) + 1;
            _circulars.Add(circular);
        }

        public static void AddAssignment(SignatureAssignment assignment)
        {
            assignment.AssignmentId = _assignments.Max(a => a.AssignmentId) + 1;
            _assignments.Add(assignment);
        }

        public static void UpdateCustomer(Customer customer)
        {
            var index = _customers.FindIndex(c => c.CustomerId == customer.CustomerId);
            if (index != -1)
                _customers[index] = customer;
        }

        public static void UpdateContract(Contract contract)
        {
            var index = _contracts.FindIndex(c => c.ContractId == contract.ContractId);
            if (index != -1)
                _contracts[index] = contract;
        }

        public static void UpdateCircular(SignatureCircular circular)
        {
            var index = _circulars.FindIndex(s => s.CircularId == circular.CircularId);
            if (index != -1)
                _circulars[index] = circular;
        }

        public static void UpdateAssignment(SignatureAssignment assignment)
        {
            var index = _assignments.FindIndex(a => a.AssignmentId == assignment.AssignmentId);
            if (index != -1)
                _assignments[index] = assignment;
        }

        // Test Helper Methods
        public static void ResetMockData()
        {
            InitializeMockData();
        }

        public static void ClearAllData()
        {
            _customers.Clear();
            _contracts.Clear();
            _circulars.Clear();
            _assignments.Clear();
            _users.Clear();
        }
    }
} 