using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace DaikinNet;

public class InfluxService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _log;
    private readonly string _influxUrl;
    private readonly string _influxAuth;

    public InfluxService(HttpClient httpClient, string influxUrl, string influxAuth, ILogger log)
    {
        _httpClient = httpClient;
        _log = log;
        _influxUrl = influxUrl;
        _influxAuth = influxAuth;
    }

    public async Task SubmitDeviceData(DeviceData deviceData)
    {
        _log.Debug("Submitting device data...");

        try
        {
            var payload = $"temp_indoor,location=home,node=thermostat value={deviceData.TempIndoorFarenheit}"
                          + $"\ntemp_outdoor,location=home,node=thermostat value={deviceData.TempOutdoorFarenheit}"
                          + $"\nhum_indoor,location=home,node=thermostat value={deviceData.HumIndoor}"
                          + $"\nhum_outdoor,location=home,node=thermostat value={deviceData.HumOutdoor}"
                          + $"\nhum_outdoor,location=home,node=thermostat value={deviceData.HumOutdoor}"
                          + $"\naq_outdoor,location=home,node=thermostat value={deviceData.AqOutdoorValue}"
                          + $"\ntarget_heating_temp,location=home,node=thermostat value={deviceData.TargetHeatingTemperatureFarhenheit}"
                          + $"\ntarget_cooling_temp,location=home,node=thermostat value={deviceData.TargetCoolingTemperatureeFarhenheit}"
                          + $"\nequipment_status,location=home,node=thermostat value={(int) deviceData.EquipmentStatus}"
                          + $"\nmode,location=home,node=thermostat value={(int) deviceData.Mode}"
                          + $"\nfan_circulate,location=home,node=thermostat value={Convert.ToInt32(deviceData.FanCirculate)}"
                          + $"\nfan_circulate_speed,location=home,node=thermostat value={(int) deviceData.FanCirculateSpeed}"
                          + $"\nfan_circulate_speed,location=home,node=thermostat value={(int) deviceData.CTCompressorCurrent}"
                          + $"\nfan_circulate_speed,location=home,node=thermostat value={(int) deviceData.CTControlAlgorithmCoolDemand}"
                          + $"\nfan_circulate_speed,location=home,node=thermostat value={(int) deviceData.CTControlAlgorithmRawDemand}";
            
            var content = new StringContent(payload, Encoding.UTF8);

            // wire basic auth
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", _influxAuth);

            var response = await _httpClient.PostAsync(_influxUrl, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            LogError("Error submitting device data:", e);
        }
    }

    private void LogError(string message, Exception e)
    {
        _log.Error(message, e);
    }
}