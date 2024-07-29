using DaikinNet;
using Newtonsoft.Json;

class Program
{
    enum ProgramMode
    {
        DEFAULT,
        EXPORT_JSON
    }
    
    public static void Main(string[] args)
    {
        var mode = ProgramMode.DEFAULT;
        
        // parse args
        if (args.Length == 1)
        {
            switch (args[0].ToLower())
            {
                case "export_json":
                    mode = ProgramMode.EXPORT_JSON;
                    break;
                default:
                    mode = ProgramMode.DEFAULT;
                    break;
            }
        }
        
        // await Go
        new Program().Go(mode).Wait();
    }

    private async Task Go(ProgramMode mode)
    {
        if (mode == ProgramMode.DEFAULT)
        {
            await RunDefaultMode();
        }
        else if (mode == ProgramMode.EXPORT_JSON)
        {
            await RunExportJsonMode();
        }
    }

    private async Task RunExportJsonMode()
    {
        // read environment variables for username and password
        var username = Environment.GetEnvironmentVariable("DAIKIN_USERNAME");
        var password = Environment.GetEnvironmentVariable("DAIKIN_PASSWORD");
        
        // assert username and password are not-null and not-empty
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Please set DAIKIN_USERNAME and DAIKIN_PASSWORD environment variables.");
            return;
        }

        var authHttpClient = new HttpClient();
        var authService = new AuthService(authHttpClient, username, password, new ConsoleLogger());

        var deviceHttpClient = new HttpClient();
        var deviceService = new DeviceService(deviceHttpClient, authService, new ConsoleLogger());

        // get device list
        var deviceList = await deviceService.GetDeviceList();

        if (deviceList.Devices != null)
        {
            foreach (var device in deviceList.Devices)
            {
                var deviceData = await deviceService.GetRawDeviceData(device.Id);
                Console.WriteLine($" - Device: {device.Name}");
                Console.WriteLine(deviceData?.ToString());
            } 
        }
    }

    private async Task RunDefaultMode()
    {
        // read environment variables for username and password
        var username = Environment.GetEnvironmentVariable("DAIKIN_USERNAME");
        var password = Environment.GetEnvironmentVariable("DAIKIN_PASSWORD");
        
        // read influx environment variables
        var influxUrl = Environment.GetEnvironmentVariable("INFLUX_URL");
        var influxAuth = Environment.GetEnvironmentVariable("INFLUX_AUTH");
        
        // read poll interval from environment variables as an integer
        var pollInterval = int.Parse(Environment.GetEnvironmentVariable("POLL_INTERVAL") ?? "5000");
        
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
        
        // load config
        var config = LoadConfig();
        if (config == null)
        {
            Console.WriteLine("Error loading `config.json`.");
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

        while (true)
        {
            if (deviceList?.Devices != null)
                foreach (var device in deviceList?.Devices)
                {
                    var reportMap = new Dictionary<string, string>();

                    var rawDeviceData = await deviceService.GetRawDeviceData(device.Id, true);
                    if (rawDeviceData != null)
                    {
                        foreach (var configReportField in config.ReportFields)
                        {
                            if (rawDeviceData.ContainsKey(configReportField.SourceField.ToLower()))
                            {
                                var reportField = configReportField.ReportField;
                                if (string.IsNullOrWhiteSpace(reportField))
                                {
                                    reportField = config.ReportWithSnakeCase ? StringUtils.CamelCaseToSnakeCase(configReportField.SourceField) : configReportField.SourceField;
                                }

                                var fieldValue = rawDeviceData[configReportField.SourceField.ToLower()]?.ToString();
                                if (!string.IsNullOrWhiteSpace(fieldValue))
                                {
                                    reportMap.Add(reportField, fieldValue);
                                }
                            }
                        }
                    }

                    if (reportMap.Count > 0)
                    {
                        await influxService.SubmitMap(reportMap);
                    }
                }

            Thread.Sleep(pollInterval);
        }
    }

    private Config? LoadConfig()
    {
        if (File.Exists("config.json"))
        {
            var configJson = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(configJson);
            return config;
        }

        Console.WriteLine("Config.json not found!");
        return null;
    }
}
