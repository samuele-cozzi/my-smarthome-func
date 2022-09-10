namespace home.api.models;

public class HomeIotHubAirConditioner {
    public int power { get; set; }
    public int temperature { get; set; }
    public string mode { get; set; }
    public int fan { get; set; }
    public int interval { get; set; }
    public int enabled { get; set; }
}