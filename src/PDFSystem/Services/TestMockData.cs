using System;
using System.Threading.Tasks;
using PDFSystem.Models;

namespace PDFSystem.Services
{
    public class TestMockData
    {
        public static async Task RunTestsAsync()
        {
            try
            {
                Console.WriteLine("Mock Data Test Başlıyor...\n");

                // Customer Service Tests
                var customerService = new CustomerService(useMockData: true);
                
                Console.WriteLine("1. Tüm Aktif Müşterileri Listele:");
                var activeCustomers = await customerService.GetCustomersAsync("Aktif");
                foreach (var customer in activeCustomers)
                {
                    Console.WriteLine($"- {customer.CustomerName} ({customer.Address})");
                }

                // Contract Service Tests
                var contractService = new ContractService(useMockData: true);
                
                Console.WriteLine("\n2. ABC Teknoloji'nin (ID:1) Kontratlarını Listele:");
                var contracts = await contractService.GetContractsAsync(customerId: 1);
                foreach (var contract in contracts)
                {
                    Console.WriteLine($"- {contract.ContractNo}: {contract.ContractType} ({contract.Status})");
                }

                // Signature Circular Tests
                var circularService = new SignatureCircularService(useMockData: true);
                
                Console.WriteLine("\n3. Mevcut İmza Sirkülerleri:");
                var circulars = await circularService.GetCircularsAsync();
                foreach (var circular in circulars)
                {
                    Console.WriteLine($"- Sirküler #{circular.CircularId}: {circular.Description}");
                    
                    // Her sirküler için imza atamalarını göster
                    var assignments = await circularService.GetAssignmentsAsync(circular.CircularId);
                    foreach (var assignment in assignments)
                    {
                        var user = MockDataService.GetUserById(assignment.UserId);
                        Console.WriteLine($"  * İmzacı: {user.UserName} - Durum: {assignment.Status}");
                    }
                }

                // Yeni Müşteri Ekleme Testi
                Console.WriteLine("\n4. Yeni Müşteri Ekleme Testi:");
                var newCustomer = new Customer
                {
                    CustomerName = "Test Şirketi Ltd.",
                    Status = "Aktif",
                    Address = "Test Adresi",
                    Phone = "0555 555 5555"
                };
                await customerService.AddCustomerAsync(newCustomer);
                Console.WriteLine($"- Yeni müşteri eklendi: {newCustomer.CustomerName}");

                // Yeni Kontrat Ekleme Testi
                Console.WriteLine("\n5. Yeni Kontrat Ekleme Testi:");
                var newContract = new Contract
                {
                    ContractNo = "CNT-2024-TEST",
                    CustomerId = 1,
                    ContractDate = DateTime.Now,
                    ContractType = "Test Sözleşmesi",
                    Description = "Test kontratı",
                    Status = "Aktif"
                };
                await contractService.AddContractAsync(newContract);
                Console.WriteLine($"- Yeni kontrat eklendi: {newContract.ContractNo}");

                Console.WriteLine("\nMock Data Test Tamamlandı!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nHATA: {ex.Message}");
            }
        }
    }
} 