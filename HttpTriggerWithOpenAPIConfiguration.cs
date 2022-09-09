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
    public class HttpTriggerWithOpenAPIConfiguration
    {
        private readonly ILogger<HttpTriggerWithOpenAPIConfiguration> _logger;

        public HttpTriggerWithOpenAPIConfiguration(ILogger<HttpTriggerWithOpenAPIConfiguration> log)
        {
            _logger = log;
        }

        [FunctionName("HttpTriggerWithOpenAPIConfiguration")]
        [StorageAccount("AzureStateStorage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(HomeConfiguration))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [Blob("home-state/home_configuration", FileAccess.Read)] string reader,
            [Blob("home-state/home_configuration", FileAccess.Write)] TextWriter writer,
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post", Route = null)] HttpRequest req
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (req.Method == HttpMethod.Get.ToString()){
                HomeConfiguration data = JsonConvert.DeserializeObject<HomeConfiguration>(reader);
                await writer.WriteLineAsync(JsonConvert.SerializeObject(data));
                return new OkObjectResult(reader);
            }
            else if (req.Method == HttpMethod.Post.ToString()){
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                HomeConfiguration data = JsonConvert.DeserializeObject<HomeConfiguration>(requestBody);
                await writer.WriteLineAsync(JsonConvert.SerializeObject(data));
                return new OkObjectResult("OK");
            }
            else {

                return new NotFoundObjectResult("Method not found");
            }
        }
    }
}

