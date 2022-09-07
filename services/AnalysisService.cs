namespace home.api.services;

public class AnalysisService : IAnalysisService{
    
    private readonly ILogger logger;
    private readonly IOptions<DaprSettings> settings;
    
    public AnalysisService(IOptions<DaprSettings> daprSettings, ILoggerFactory loggerFactory){
        logger = loggerFactory.CreateLogger("Start");
        settings = daprSettings;
    }

    private List<HomeAnalysis> map(Home home){

        List<HomeAnalysis> result = new List<HomeAnalysis>();

        List<string> devices = new List<string>();
        foreach (var room in home.Configuration.Rooms){
            foreach (var device in room.Devices){
                if (!devices.Contains(device.Id)) devices.Add(device.Id);
            }
        }

        foreach (var deviceId in devices){
            var thermostat = home.Thermostats.FirstOrDefault(x => x.deviceId == deviceId);
            var airConditioner = home.AirConditioners.FirstOrDefault(x => x.DeviceId == deviceId);

            result.Add(new HomeAnalysis(){
                DeviceId = deviceId,
                Timestamp = DateTime.Now,
                HeatIndex = thermostat?.heatIndex,
                Humidity = thermostat?.humidity,
                Temperature = thermostat?.temperature,
                TargetHeatIndex = home.Configuration.TargetTemperature,
                TargetHumidity = home.Configuration.TargetHumidity,
                TargetTemperature = home.Configuration.TargetTemperature,
                ACPower = airConditioner?.Power,
                ACMode = airConditioner?.Mode,
                ACTemp = airConditioner?.Temp,
                ACFan = airConditioner?.Fan
            });
        }

        return result;
    }

    public async void Save(Home home){

        var analysis = this.map(home);
        logger.LogInformation($"Save Analysis: {JsonSerializer.Serialize(analysis)}");

        using( HttpClient client = new HttpClient() ){
            var response = await client.PostAsJsonAsync<List<HomeAnalysis>>(settings.Value.PowerBIUrl, analysis);
            logger.LogInformation($"{response.StatusCode}");
        } 
    }
}