namespace home.api.services;

public class HomeServices: IHomeService {

    private readonly ILogger logger;
    private readonly IOptions<DaprSettings> settings;
    private readonly DaprClient client;
    
    public HomeServices(IOptions<DaprSettings> daprSettings, ILoggerFactory loggerFactory){
        logger = loggerFactory.CreateLogger("Start");
        settings = daprSettings;
        client = new DaprClientBuilder().Build();
    }

    public async Task<Home> Get(){

        var home = new Home();
        home.AirConditioners = new List<AirConditioner>();
        home.Thermostats = new List<Thermostat>();
        var configuration = await client.GetStateAsync<HomeConfiguration>(
            settings.Value.StateStoreName, settings.Value.StateHomeConfiguration);

        home.Configuration = configuration;

        foreach (var room in configuration.Rooms){
            foreach (var device in room.Devices){
                if(home.AirConditioners.Count(x => x.DeviceId == device.Id) == 0) {
                    var airConditioner = await client.GetStateAsync<AirConditioner>(
                        settings.Value.StateStoreName, $"{settings.Value.StateAirConditioner}/{device.Id}");
                    if(airConditioner != null) home.AirConditioners.Add(airConditioner);
                }

                if (home.Thermostats.Count(x => x.deviceId == device.Id) == 0) {
                    var thermostat = await client.GetStateAsync<Thermostat>(
                        settings.Value.StateStoreName, $"{settings.Value.StateThermostat}/{device.Id}");
                    if(thermostat != null) home.Thermostats.Add(thermostat);
                }
                
            }
        }

        return home;
    }

    public async void SaveArchive(Home home){

        logger.LogInformation($"Archive Home: {JsonSerializer.Serialize(home)}");

        await client.SaveStateAsync<Home>(
            settings.Value.StateStoreName, 
            $"history/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}/{DateTime.Now.ToString("HH:mm:ss")}-{settings.Value.StateHome}", 
            home
        );   
    }

    public async void SaveAirConditioner(AirConditioner airConditioner){

        logger.LogInformation($"Save Air Conditioner: {JsonSerializer.Serialize(airConditioner)}");

        await client.SaveStateAsync<AirConditioner>(
            settings.Value.StateStoreName, $"{settings.Value.StateAirConditioner}/{airConditioner.DeviceId}", airConditioner
        );   
    }

    public async void SaveThermostat(Thermostat thermostat){

        logger.LogInformation($"Save Thermostat: {JsonSerializer.Serialize(thermostat)}");

        await client.SaveStateAsync<Thermostat>(
            settings.Value.StateStoreName, $"{settings.Value.StateThermostat}/{thermostat.deviceId}", thermostat
        );   
    }
}