namespace home.api.services.interfaces;

public interface IConfigurationService {
    public Task<HomeConfiguration> Get();
    public void Save(HomeConfiguration conf);
}