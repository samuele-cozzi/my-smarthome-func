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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("home-state/ac-controller/{DeviceId}", FileAccess.Write)] string stateACController
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            AirConditioner data = JsonConvert.DeserializeObject<AirConditioner>(requestBody);
            stateACController = JsonConvert.SerializeObject(data);
            return new OkObjectResult(stateACController);
        }
    }
}

