using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Newtonsoft.Json;

namespace SmartHome.Functions
{
    
    public class IoTHubTriggerThermostatDevice
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("IoTHubTriggerThermostatDevice")]
        [StorageAccount("AzureStateStorage")]
        public void Run(
            [IoTHubTrigger("iothub-ehub-iot-smarth-21108025-eb589840ae", Connection = "AzureIotHub")]EventData message, 
            [Blob("home-state/home", FileAccess.ReadWrite)] string stateHome,
            [Blob("home-state/thermostat/{SystemProperties.iothub-connection-device-id}", FileAccess.Write)] out string stateThermostat,
            [Blob("home-state/hystory/{enqueuedTimeUtc.Year}/{enqueuedTimeUtc.Month}/{enqueuedTimeUtc.Day}/{enqueuedTimeUtc.Hour}:{enqueuedTimeUtc.Minute}-home", FileAccess.Write)] out string stateHistory,
            DateTime enqueuedTimeUtc,
            Int64 sequenceNumber,
            string offset,
            ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

            // 1. Logging input

            log.LogInformation($"Event: {Encoding.UTF8.GetString(message.Body)}");
            // Metadata accessed by binding to EventData
            log.LogInformation($"EnqueuedTimeUtc={message.SystemProperties.EnqueuedTimeUtc}");
            log.LogInformation($"SequenceNumber={message.SystemProperties.SequenceNumber}");
            log.LogInformation($"Offset={message.SystemProperties.Offset}");
            // Metadata accessed by using binding expressions in method parameters
            log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
            log.LogInformation($"SequenceNumber={sequenceNumber}");
            log.LogInformation($"Offset={offset}");
            log.LogInformation($"DeviceID={message.SystemProperties["iothub-connection-device-id"].ToString()}");
            string DeviceID = message.SystemProperties["iothub-connection-device-id"].ToString();

            // 2. Write Thermostat

            var thermostatIoT = JsonConvert.DeserializeObject<HomeIotHub>(Encoding.UTF8.GetString(message.Body));

            var thermostat = new Thermostat(){
                deviceId = DeviceID,
                heatIndex = (double )thermostatIoT.heatIndex / thermostatIoT.factor,
                humidity = (double )thermostatIoT.humidity / thermostatIoT.factor,
                temperature = (double )thermostatIoT.temperature / thermostatIoT.factor
            };

            stateThermostat = JsonConvert.SerializeObject(thermostat);

            log.LogInformation($"Thermostat Saved");

            // 3. Write Home

            var home = JsonConvert.DeserializeObject<Home>(stateHome);

            home.Thermostats.RemoveAll(x => x.deviceId == DeviceID);
            home.Thermostats.Add(thermostat);
            
            stateHome = JsonConvert.SerializeObject(home);

            log.LogInformation($"Home Saved");

            // 4. Write History
            
            stateHistory = stateHome;

            log.LogInformation($"History Saved");

            // 5. Write Analytics

            List<HomeAnalysis> analysis = new List<HomeAnalysis>();

            List<string> devices = new List<string>();
            foreach (var room in home.Configuration.Rooms){
                foreach (var device in room.Devices){
                    if (!devices.Contains(device.Id)) devices.Add(device.Id);
                }
            }

            foreach (var deviceId in devices){
                var thermostatAnalytics = home.Thermostats.FirstOrDefault(x => x.deviceId == deviceId);
                var airConditionerAnalytics = home.AirConditioners.FirstOrDefault(x => x.DeviceId == deviceId);

                analysis.Add(new HomeAnalysis(){
                    DeviceId = deviceId,
                    Timestamp = DateTime.Now,
                    HeatIndex = thermostatAnalytics?.heatIndex,
                    Humidity = thermostatAnalytics?.humidity,
                    Temperature = thermostatAnalytics?.temperature,
                    TargetHeatIndex = home.Configuration.TargetTemperature,
                    TargetHumidity = home.Configuration.TargetHumidity,
                    TargetTemperature = home.Configuration.TargetTemperature,
                    ACPower = airConditionerAnalytics?.Power,
                    ACMode = airConditionerAnalytics?.Mode,
                    ACTemp = airConditionerAnalytics?.Temp,
                    ACFan = airConditionerAnalytics?.Fan
                });
            }

            var response = client.PostAsJsonAsync<List<HomeAnalysis>>(Environment.GetEnvironmentVariable("PowerBIUrl"), analysis).Result;
            log.LogInformation($"{response.StatusCode}");
        }
    }
}