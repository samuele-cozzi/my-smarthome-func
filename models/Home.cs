namespace home.api.models;

public class Home {
    public HomeConfiguration Configuration { get; set; } 
    public List<AirConditioner> AirConditioners { get; set; }
    public List<Thermostat> Thermostats { get; set; }
}