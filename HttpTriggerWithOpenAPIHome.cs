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
    public class HttpTriggerWithOpenAPIHome
    {
        private readonly ILogger<HttpTriggerWithOpenAPIHome> _logger;

        public HttpTriggerWithOpenAPIHome(ILogger<HttpTriggerWithOpenAPIHome> log)
        {
            _logger = log;
        }

        [FunctionName("HttpTriggerWithOpenAPIHome")]
        [StorageAccount("AzureStateStorage")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Home))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Blob("home-state/home", FileAccess.Read)] string reader,
            [Blob("home-state/home", FileAccess.Write)] TextWriter writer
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
                Home data = JsonConvert.DeserializeObject<Home>(requestBody);
                await writer.WriteLineAsync(JsonConvert.SerializeObject(data));
                return new OkObjectResult("OK");
            }
            else {
                _logger.LogError("Method not found");
                return new NotFoundObjectResult("Method not found");
            }
        }
    }
}

