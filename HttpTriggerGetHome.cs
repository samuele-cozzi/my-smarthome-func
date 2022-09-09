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
    public class HttpTriggerGetHome
    {
        private readonly ILogger<HttpTriggerGetHome> _logger;

        public HttpTriggerGetHome(ILogger<HttpTriggerGetHome> log)
        {
            _logger = log;
        }

        [FunctionName("HttpTriggerGetHome")]
        [StorageAccount("AzureStateStorage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("home-state/home", FileAccess.Read)] string reader
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(reader);
        
        }
    }
}

