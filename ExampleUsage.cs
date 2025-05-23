// Örnek model sınıfı
public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public CustomerStatus Status { get; set; }
}

public enum CustomerStatus
{
    Active = 1,
    Inactive = 2,
    Blocked = 3
}

// Örnek kullanım
public class CustomerService
{
    private readonly DatabaseHelper _db;

    public CustomerService(string connectionString)
    {
        _db = new DatabaseHelper(connectionString);
    }

    // SQL sorgusu ile liste alma
    public async Task<List<Customer>> GetCustomersByStatusAsync(CustomerStatus status)
    {
        var sql = @"SELECT CustomerId, Name, Email, LastOrderDate, Status 
                    FROM Customers 
                    WHERE Status = @Status 
                    ORDER BY Name";

        var parameters = new Dictionary<string, object>
        {
            { "@Status", status }
        };

        return await _db.ExecuteList<Customer>(sql, parameters);
    }

    // Stored Procedure ile liste alma
    public async Task<List<Customer>> GetActiveCustomersAsync()
    {
        var parameters = new Dictionary<string, object>
        {
            { "@Status", CustomerStatus.Active },
            { "@LastOrderDate", DateTime.Now.AddMonths(-6) }
        };

        return await _db.ExecuteSpList<Customer>("sp_GetActiveCustomers", parameters);
    }

    // Tek kayıt alma (SQL sorgusu ile)
    public async Task<Customer> GetCustomerByEmailAsync(string email)
    {
        var sql = "SELECT * FROM Customers WHERE Email = @Email";
        var parameters = new Dictionary<string, object>
        {
            { "@Email", email }
        };

        return await _db.ExecuteSingle<Customer>(sql, parameters);
    }

    // Tek kayıt alma (Stored Procedure ile)
    public async Task<Customer> GetCustomerByIdAsync(int customerId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@CustomerId", customerId }
        };

        return await _db.ExecuteSpSingle<Customer>("sp_GetCustomerById", parameters);
    }

    // Scalar değer alma örneği
    public async Task<int> GetCustomerCountAsync(CustomerStatus status)
    {
        var sql = "SELECT COUNT(*) FROM Customers WHERE Status = @Status";
        var parameters = new Dictionary<string, object>
        {
            { "@Status", status }
        };

        return await _db.ExecuteScalar<int>(sql, parameters);
    }
} 