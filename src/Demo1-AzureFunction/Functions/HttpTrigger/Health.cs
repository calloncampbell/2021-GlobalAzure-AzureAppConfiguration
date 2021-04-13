using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace Demo1_AzureFunction.Functions
{
    public static class Health
    {
        [FunctionName(nameof(Health))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"HTTP trigger function processed a request for Health");


            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var functionRuntimeVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var regionName = Environment.GetEnvironmentVariable("REGION_NAME");

            var appMessage = $"Health check for {appName}!";
            var responseMessage = new
            {
                Message = appMessage.Trim(),
                ApplicationName = appName,
                ApplicationVersion = appVersion,
                Region = regionName,
                FunctionRuntimeVersion = functionRuntimeVersion,
                CurrentDatetime = DateTime.UtcNow,
            };

            return new OkObjectResult(responseMessage);
        }
    }
}

