using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;

namespace Demo1_AzureFunction.Functions
{
    public class CheckFeatures
    {
        private readonly IConfigurationRefresher _configurationRefresher;
        private readonly IFeatureManager _featureManager;

        public CheckFeatures(
            IConfigurationRefresher configurationRefresher,
            IFeatureManager featureManager)
        {
            _configurationRefresher = configurationRefresher;
            _featureManager = featureManager;
        }

        [FunctionName(nameof(CheckFeatures))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await _configurationRefresher.TryRefreshAsync();

            string message = await _featureManager.IsEnabledAsync("Beta")
                ? "The Feature Flag 'Beta' is turned ON"
                : "The Feature Flag 'Beta' is turned OFF";

            return (ActionResult)new OkObjectResult(message);
        }
    }
}

