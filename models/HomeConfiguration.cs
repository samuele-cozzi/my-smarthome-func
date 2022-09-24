namespace home.api.models;

public class HomeConfiguration {
    public int TargetHumidity { get; set; }
    public int TargetTemperature { get; set; }
    public int TemperatureTolerance { get; set; }
    public int IntervalThermostat { get; set; }
    public bool Enabled { get; set; }
    public string Mode { get; set; }
    public string AirMode { get; set; }
    public int AirFan { get; set; }
    public int AirTemperature { get; set; }
    public List<HomeRoom> Rooms { get; set; }
}