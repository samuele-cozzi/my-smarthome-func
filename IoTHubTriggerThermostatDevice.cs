using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;

namespace SmartHome.Function
{
    public class IoTHubTriggerThermostatDevice
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("IoTHubTriggerThermostatDevice")]
        public void Run([IoTHubTrigger("iothub-ehub-iot-smarth-21108025-eb589840ae", Connection = "AzureIotHub")]EventData message, 
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
        }
    }
}