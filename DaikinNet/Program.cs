using DaikinNet;

// read environment variables for username and password
var username = Environment.GetEnvironmentVariable("DAIKIN_USERNAME");
var password = Environment.GetEnvironmentVariable("DAIKIN_PASSWORD");

// read influx environment variables
var influxUrl = Environment.GetEnvironmentVariable("INFLUX_URL");
var influxAuth = Environment.GetEnvironmentVariable("INFLUX_AUTH");

// assert username and password are not-null and not-empty
if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
{
    Console.WriteLine("Please set DAIKIN_USERNAME and DAIKIN_PASSWORD environment variables.");
    return;
}

// assert influx environment variables are not-null and not-empty
if (string.IsNullOrWhiteSpace(influxUrl) || string.IsNullOrWhiteSpace(influxAuth))
{
    Console.WriteLine("Please set INFLUX_URL and INFLUX_AUTH environment variables.");
    return;
}

var authHttpClient = new HttpClient();
var authService = new AuthService(authHttpClient, username, password, new ConsoleLogger());

var deviceHttpClient = new HttpClient();
var deviceService = new DeviceService(deviceHttpClient, authService, new ConsoleLogger());

var influxHttpClient = new HttpClient();
var influxService = new InfluxService(influxHttpClient, influxUrl, influxAuth, new ConsoleLogger());

// get device list
var deviceList = await deviceService.GetDeviceList();

// if (deviceList?.Devices != null)
// {
//     var device = deviceList.Devices.FirstOrDefault();
//     deviceService.SetTemps(device.Id, 21.2, 22.8);
// }
//
// return;
while (true)
{
    foreach (var device in deviceList.Devices)
    {
        var deviceData = await deviceService.GetDeviceData(device.Id);
        Console.WriteLine($" - Device: {device.Name}, Data: \n{deviceData?.ToString()}");
        
        if (deviceData != null) await influxService.SubmitDeviceData(deviceData);
    }
    
    Thread.Sleep(5000);
}
