namespace home.api.services.interfaces;

public interface IHomeService {
    public Task<Home> Get();
    public void SaveArchive(Home home);
    public void SaveAirConditioner(AirConditioner airConditioner);
    public void SaveThermostat(Thermostat thermostat);
}