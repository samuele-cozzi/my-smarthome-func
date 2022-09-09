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
            [Blob("home-state/triage/thermostat/{enqueuedTimeUtc}-{sequenceNumber}", FileAccess.Write)] out string state,
            DateTime enqueuedTimeUtc,
            Int64 sequenceNumber,
            string offset,
            ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

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

            var thermostat = JsonConvert.DeserializeObject<HomeIotHub>(Encoding.UTF8.GetString(message.Body));

            state = JsonConvert.SerializeObject(new Thermostat(){
                deviceId = message.SystemProperties["iothub-connection-device-id"].ToString(),
                heatIndex = (double )thermostat.heatIndex / thermostat.factor,
                humidity = (double )thermostat.humidity / thermostat.factor,
                temperature = (double )thermostat.temperature / thermostat.factor
            });
        }
    }
}