namespace home.api.models;

public class HomeConfiguration {
    public int TargetHumidity { get; set; }
    public int TargetTemperature { get; set; }
    public List<HomeRoom> Rooms { get; set; }
}