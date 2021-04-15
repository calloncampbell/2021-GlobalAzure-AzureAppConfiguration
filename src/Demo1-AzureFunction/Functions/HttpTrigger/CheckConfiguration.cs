using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo1_AzureFunction.Functions
{
    public class CheckConfiguration
    {
        private readonly IConfigurationRoot _configuration;
        private const string ConfigPrefix = "DemoFunction:";

        public CheckConfiguration(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        [FunctionName(nameof(CheckConfiguration))]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var responseMessage = new
            {                
                sqlCommandTimeoutInSeconds = int.Parse(_configuration[$"{ConfigPrefix}SqlCommandTimeout"] ?? "30"),
                queueProcessingDelayTimeoutInSeconds = int.Parse(_configuration[$"{ConfigPrefix}QueueProcessingDelayTimeout"] ?? "30"),
                workItemQueueName = _configuration[$"{ConfigPrefix}WorkItemsQueueName"],
                httpClientTimeoutInSeconds = int.Parse(_configuration[$"{ConfigPrefix}HttpClientTimeoutInSeconds"] ?? "100"),
                geoStorageMaxRetries = int.Parse(_configuration[$"{ConfigPrefix}GeoRedundantStorage-MaxRetries"] ?? "3"),
                geoStorageDelayInSeconds = double.Parse(_configuration[$"{ConfigPrefix}GeoRedundantStorage-DelayInSeconds"] ?? "0.5"),
                geoStorageMaxDelayInSeconds = int.Parse(_configuration[$"{ConfigPrefix}GeoRedundantStorage-MaxDelayInSeconds"] ?? "3")
            };

            return new OkObjectResult(responseMessage);
        }
    }
}

