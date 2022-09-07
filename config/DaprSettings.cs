namespace home.api.config;

public class DaprSettings
{
    public string StateStoreName { get; set; }
    public string StateHome { get; set; }
    public string StateHomeConfiguration { get; set; }
    public string StateAirConditioner { get; set; }
    public string StateThermostat { get; set; }
    public string PubSubName { get; set; }
    public string PubSubHomeEnvironmentTopicName { get; set; }
    public string PowerBIUrl { get; set; }

}