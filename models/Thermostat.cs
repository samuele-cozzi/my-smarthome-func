namespace home.api.models;

public class Thermostat {
    public string deviceId { get; set; }
    public double temperature { get; set; }
    public double humidity { get; set; }
    public double heatIndex { get; set; }
}