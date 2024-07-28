using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DaikinNet;

public class DeviceService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ILogger _log;
    private TokenResponse? _tokenResponse;
    private DateTime _tokenExpirationDate;

    public DeviceService(HttpClient httpClient, AuthService authService, ILogger log)
    {
        _httpClient = httpClient;
        _authService = authService;
        _log = log;
        
        // configure httpClient
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<DeviceListResponse?> GetDeviceList()
    {
        try
        {
            await CheckAuth();
            
            _log.Debug("Getting device list...");
            
            var response = await _httpClient.GetAsync("https://api.daikinskyport.com/devices");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            
            var devicesResponse = JsonConvert.DeserializeObject<List<Device>>(responseBody);
            return new DeviceListResponse() { Devices = devicesResponse};
        }
        catch (HttpRequestException e)
        {
            LogError("Error getting device list:", e);
        }
        
        return null;
    } 
    
    public async Task<DeviceData?> GetDeviceData(string deviceId)
    {
        try
        {
            await CheckAuth();

            _log.Debug("Getting device data...");
            
            var response = await _httpClient.GetAsync($"https://api.daikinskyport.com/deviceData/{deviceId}");
            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync();

            var deviceDataResponse = JsonConvert.DeserializeObject<DeviceData>(responseBodyJson);
            return deviceDataResponse;
        }
        catch (HttpRequestException e)
        {
            LogError("Error getting device list:", e);
        }
        
        return null;
    }

    public async Task<JObject?> GetRawDeviceData(string deviceId, bool toLowerKeys = false)
    {
        try
        {
            await CheckAuth();

            _log.Debug("Getting device data...");
            
            var response = await _httpClient.GetAsync($"https://api.daikinskyport.com/deviceData/{deviceId}");
            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync();

            var jObj = JObject.Parse(responseBodyJson);
            
            var sortedJsonObject = SortProperties(jObj, toLowerKeys);
            // var sortedJson = sortedJsonObject.ToString();
            return sortedJsonObject;
        }
        catch (HttpRequestException e)
        {
            LogError("Error getting device list:", e);
        }
        
        return null;
    }
    
    private async Task CheckAuth()
    {
        // get token
        if (_tokenResponse == null)
        {
            _tokenResponse = await _authService.GetTokenAsync();
                
            // set token expiration date
            if (_tokenResponse != null)
            {
                // set token expiration to 5 minutes because their actual expiration doesn't work
                _tokenExpirationDate = DateTime.Now.AddSeconds(300);
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_tokenResponse?.AccessToken}");
        }
            
        // check for expired token
        if (_tokenExpirationDate <= DateTime.Now)
        {
            _log.Debug("Token expired, getting new token...");
            _tokenResponse = null;
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            await CheckAuth();
        }
    }

    private void LogError(string message, Exception e)
    {
        _log.Error(message, e);
    }

    public async Task SetTemps(string deviceId, double coolTarget, double heatTarget)
    {
        await CheckAuth();
        
        var payload = new
        {
            hspHome = coolTarget,
            cspHome = heatTarget
        };

        var jObj = JsonConvert.SerializeObject(payload);
        _log.Debug($"Set temps payload: {jObj}");
        
        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            
        // set content type
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        // set accepts header
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _httpClient.PutAsync($"https://api.daikinskyport.com/deviceData/{deviceId}", content);

        var responseBody = await response.Content.ReadAsStringAsync();
            
        _log.Debug($"Set temps response: {responseBody}");
    }
    
    static JObject SortProperties(JObject original, bool toLowerKeys)
    {
        var sortedProperties = new SortedDictionary<string, JToken>();
        foreach (var property in original.Properties())
        {
            if (toLowerKeys)
            {
                sortedProperties.Add(property.Name.ToLower(), property.Value);
            }
            else
            {
                sortedProperties.Add(property.Name, property.Value);
            }
        }

        JObject sortedObject = new JObject();
        foreach (var property in sortedProperties)
        {
            var key = toLowerKeys ? property.Key.ToLower() : property.Key;
            
            if (property.Value is JObject nestedObject)
            {
                sortedObject.Add(key, SortProperties(nestedObject, toLowerKeys));
            }
            else
            {
                sortedObject.Add(key, property.Value);
            }
        }

        return sortedObject;
    }

}