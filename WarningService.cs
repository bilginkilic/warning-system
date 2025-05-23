using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WarningService
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public WarningService(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    public async Task<List<Warning>> GetWarningsAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/warnings");
            return JsonConvert.DeserializeObject<List<Warning>>(response);
        }
        catch (Exception ex)
        {
            throw new Exception("Uyarılar alınırken bir hata oluştu: " + ex.Message);
        }
    }
} 