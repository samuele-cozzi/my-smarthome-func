namespace home.api.models;

public class HomeIotHubThermostat {
    public int msgCount { get; set; }
    public int temperature { get; set; }
    public int humidity { get; set; }
    public int heatIndex { get; set; }
    public int factor { get; set; }
}