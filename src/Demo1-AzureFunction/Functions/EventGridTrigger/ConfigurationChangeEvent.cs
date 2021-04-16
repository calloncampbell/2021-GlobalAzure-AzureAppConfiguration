// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Demo1_AzureFunction.Functions
{
    public class ConfigurationChangeEvent
    {
        private IConfigurationRefresher _configurationRefresher;

        public ConfigurationChangeEvent(IConfigurationRefresher configurationRefresher)
        {
            _configurationRefresher = configurationRefresher;
        }

        [FunctionName(nameof(ConfigurationChangeEvent))]
        public void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            _configurationRefresher.SetDirty();
        }
    }
}
