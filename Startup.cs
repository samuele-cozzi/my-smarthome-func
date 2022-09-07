using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(SmartHome.Functions.Startup))]

namespace SmartHome.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddControllers().AddDapr();
            builder.Services.Configure<DaprSettings>(builder.Configuration.GetSection(nameof(DaprSettings)));

            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
            builder.Services.AddScoped<IAnalysisService, AnalysisService>();
            builder.Services.AddScoped<IHomeService, HomeServices>();
        }
    }
}