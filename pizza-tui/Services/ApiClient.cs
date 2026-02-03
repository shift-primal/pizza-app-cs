using System.Net.Http.Json;

public class ApiClient
{
    private readonly HttpClient _client = new();
    private const string BaseUrl = "http://localhost:5145";

    public ApiClient()
    {
        _client.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var res = await _client.GetAsync(endpoint);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"API error: {res.StatusCode}");

        return await res.Content.ReadFromJsonAsync<T>()
            ?? throw new HttpRequestException("API error: Result is null");
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var res = await _client.PostAsJsonAsync(endpoint, data);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"API error: {res.StatusCode}");

        return await res.Content.ReadFromJsonAsync<TResponse>()
            ?? throw new HttpRequestException("API error: Result is null");
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var res = await _client.PutAsJsonAsync(endpoint, data);
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException($"API error: {res.StatusCode}");

        return await res.Content.ReadFromJsonAsync<TResponse>()
            ?? throw new HttpRequestException("API error: Result is null");
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var res = await _client.DeleteAsync(endpoint);
        return res.IsSuccessStatusCode;
    }
}
