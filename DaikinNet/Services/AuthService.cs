using System.Text;
using Newtonsoft.Json;

namespace DaikinNet;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _user;
    private readonly string _password;
    private readonly ILogger _log;

    public AuthService(HttpClient httpClient, string user, string password, ILogger log)
    {
        _httpClient = httpClient;
        _user = user;
        _password = password;
        _log = log;
    }

    public async Task<TokenResponse?> GetTokenAsync()
    {
        _log.Debug("Getting token...");

        var requestBody = new
        {
            email = _user,
            password = _password
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("https://api.daikinskyport.com/users/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            
            // return token
            var token = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return token;
        }
        catch (HttpRequestException e)
        {
            LogError("Error getting token:", e);
            throw new Exception("Error getting token", e);
        }
    }

    private void LogError(string message, Exception e)
    {
        _log.Error(message, e);
    }
}