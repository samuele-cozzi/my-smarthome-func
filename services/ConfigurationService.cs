namespace home.api.services;

public class ConfigurationService: IConfigurationService {

    private readonly ILogger logger;
    private readonly IOptions<DaprSettings> settings;
    private readonly DaprClient client;
    
    public ConfigurationService(IOptions<DaprSettings> daprSettings, ILoggerFactory loggerFactory){
        logger = loggerFactory.CreateLogger("Start");
        settings = daprSettings;
        client = new DaprClientBuilder().Build();
    }

    public async Task<HomeConfiguration> Get(){
        var result = await client.GetStateAsync<HomeConfiguration>(
            settings.Value.StateStoreName, settings.Value.StateHomeConfiguration);

        return result;
    }

    public async void Save(HomeConfiguration conf){

        logger.LogInformation($"Save: {JsonSerializer.Serialize(conf)}");

        await client.SaveStateAsync<HomeConfiguration>(
            settings.Value.StateStoreName, settings.Value.StateHomeConfiguration, conf
        );   
    }
}