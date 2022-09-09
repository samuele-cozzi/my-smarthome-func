using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace SmartHome.Functions
{
    public class HttpTriggerWithOpenAPIacController
    {
        private readonly ILogger<HttpTriggerWithOpenAPIacController> _logger;

        public HttpTriggerWithOpenAPIacController(ILogger<HttpTriggerWithOpenAPIacController> log)
        {
            _logger = log;
        }

        [FunctionName("HttpTriggerWithOpenAPIacController")]
        [StorageAccount("AzureStateStorage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "air-conditioner" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AirConditioner))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] AirConditioner req,
            [Blob("home-state/ac-controller/{DeviceId}", FileAccess.ReadWrite)] string stateACController
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            var serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("AzureIotHubConnectionString"));
            var commandMessage = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(req)));
                
            await serviceClient.SendAsync(req.DeviceId, commandMessage);

            _logger.LogInformation("Cloud2Device message sent!");

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //AirConditioner data = JsonConvert.DeserializeObject<AirConditioner>(requestBody);
            stateACController = JsonConvert.SerializeObject(req);

            _logger.LogInformation("Blob Storage Saved!");

            return new OkObjectResult(stateACController);
        }
    }
}

