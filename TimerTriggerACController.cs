namespace SmartHome.Functions
{
    public class TimerTriggerACController
    {
        private readonly ILogger<TimerTriggerACController> _logger;

        public TimerTriggerACController(ILogger<TimerTriggerACController> log)
        {
            _logger = log;
        }

        [FunctionName("TimerTriggerACController")]
        public void Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log,
            [Blob("home-state/home", FileAccess.Read)] string reader, 
            [Blob("home-state/home", FileAccess.Write)] TextWriter writer)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //-----------------------------------------------------------------------------

            Home home = JsonConvert.DeserializeObject<Home>(reader);

            if (home.Configuration.Enabled){
                switch(home.Configuration.Mode) 
                {
                    case "Autumn":
                        AutumnMode(home);
                        SaveHome(writer, home);
                        break;
                    case "Spring":
                        SpringMode(home);
                        SaveHome(writer, home);
                        break;
                    default:
                        SpringMode(home);
                        SaveHome(writer, home);
                        break;
                }
            }
            else
            {
                foreach (var ac in home.AirConditioners.Where(x => x.Power)){
                    SendAirConditionerCommand(home, ac.DeviceId, false);
                }

                SaveHome(writer, home);
            }


            //-----------------------------------------------------------------------------
        }

        private void AutumnMode (Home home){
            var temperature = home.Thermostats.FirstOrDefault().temperature;
            foreach (var ac in home.AirConditioners){
                if (ac.Power){
                    if (temperature >= home.Configuration.TargetTemperature){
                        SendAirConditionerCommand(home, ac.DeviceId, false);
                    }
                }
                else
                {
                    if (temperature <= home.Configuration.TargetTemperature - home.Configuration.TemperatureTolerance){
                        SendAirConditionerCommand(home, ac.DeviceId, true);
                    }
                }
            }
        }

        private void SpringMode (Home home){
            var temperature = home.Thermostats.FirstOrDefault().temperature;
            foreach (var ac in home.AirConditioners){
                if (ac.Power){
                    if (temperature <= home.Configuration.TargetTemperature){
                        SendAirConditionerCommand(home, ac.DeviceId, false);
                    }
                }
                else
                {
                    if (temperature >= home.Configuration.TargetTemperature - home.Configuration.TemperatureTolerance){
                        SendAirConditionerCommand(home, ac.DeviceId, true);
                    }
                }
            }
        }

        private async void SendAirConditionerCommand (Home home, string deviceId, bool power){
            
            var ac = home.AirConditioners.SingleOrDefault(x => x.DeviceId == deviceId);
            if (ac != null) {
                ac.Power = power;
            }

            var iotAC = new HomeIotHubAirConditioner(){
                enabled = Convert.ToInt32(home.Configuration.Enabled),
                interval = home.Configuration.IntervalThermostat,
                power = Convert.ToInt32(power),
                mode = home.Configuration.AirMode,
                temperature = home.Configuration.AirTemperature,
                fan = home.Configuration.AirFan
            };

            var serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("AzureIotHubConnectionString"));
            var commandMessage = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(iotAC)));
                
            await serviceClient.SendAsync(deviceId, commandMessage);

            _logger.LogInformation("Cloud2Device message sent!");

        } 

        

        private void SaveHome(TextWriter writer, Home home) {
            
            writer.WriteLine(JsonConvert.SerializeObject(home));

            _logger.LogInformation("Blob Storage Home Saved!");

        }
    }
    
}
