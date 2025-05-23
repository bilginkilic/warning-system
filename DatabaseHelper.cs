using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<T>> ExecuteList<T>(string sql, Dictionary<string, object> parameters = null, CommandType commandType = CommandType.Text) where T : class, new()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = commandType;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                var resultList = new List<T>();
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var properties = typeof(T).GetProperties()
                        .Where(prop => prop.CanWrite)
                        .ToDictionary(prop => prop.Name.ToLower(), prop => prop);

                    while (await reader.ReadAsync())
                    {
                        var item = new T();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i).ToLower();
                            if (properties.ContainsKey(columnName))
                            {
                                var property = properties[columnName];
                                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                if (value != null)
                                {
                                    // Enum handling
                                    if (property.PropertyType.IsEnum)
                                    {
                                        property.SetValue(item, Enum.ToObject(property.PropertyType, value));
                                    }
                                    // Nullable type handling
                                    else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                                    {
                                        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                                        property.SetValue(item, Convert.ChangeType(value, underlyingType));
                                    }
                                    // Regular type handling
                                    else
                                    {
                                        property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                                    }
                                }
                            }
                        }
                        resultList.Add(item);
                    }
                }

                return resultList;
            }
        }
    }

    // Tek kayıt döndüren versiyon
    public async Task<T> ExecuteSingle<T>(string sql, Dictionary<string, object> parameters = null, CommandType commandType = CommandType.Text) where T : class, new()
    {
        var result = await ExecuteList<T>(sql, parameters, commandType);
        return result.FirstOrDefault();
    }

    // Stored Procedure için kolaylık metotları
    public async Task<List<T>> ExecuteSpList<T>(string storedProcedure, Dictionary<string, object> parameters = null) where T : class, new()
    {
        return await ExecuteList<T>(storedProcedure, parameters, CommandType.StoredProcedure);
    }

    public async Task<T> ExecuteSpSingle<T>(string storedProcedure, Dictionary<string, object> parameters = null) where T : class, new()
    {
        return await ExecuteSingle<T>(storedProcedure, parameters, CommandType.StoredProcedure);
    }

    // Scalar değer döndüren versiyon
    public async Task<T> ExecuteScalar<T>(string sql, Dictionary<string, object> parameters = null, CommandType commandType = CommandType.Text)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = commandType;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result == DBNull.Value ? default(T) : (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
} 