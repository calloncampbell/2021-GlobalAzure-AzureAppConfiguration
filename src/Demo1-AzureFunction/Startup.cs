using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Demo1_AzureFunction.Startup))]

namespace Demo1_AzureFunction
{
    public class Startup : FunctionsStartup
    {
        private static IConfigurationRoot Configuration { get; set; }
        public IConfigurationBuilder ConfigurationBuilder { get; set; }
        private static IConfigurationRefresher ConfigurationRefresher { set; get; }

        public Startup()
        {
            ConfigurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables();

            Configuration = ConfigurationBuilder.Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Load configuration from Azure App Configuration
            ConfigurationBuilder.AddAzureAppConfiguration(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureAppConfigConnectionString");
                var cacheExpiryInSeconds = double.Parse(Environment.GetEnvironmentVariable("AzureAppConfigurationCacheExpirationTimeInSeconds") ?? "30");
                var environmentLabel = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AzureAppConfigurationEnvironmentLabel"))
                    ? Environment.GetEnvironmentVariable("AzureAppConfigurationEnvironmentLabel")
                    : LabelFilter.Null;

                // Use ".Connect(...)" for connection string, or use ".ConnectWithManagedIdentity(...) for managed identity"
                options.Connect(connectionString)
                       // Load all keys that start with `DemoFunction:`
                       .Select("DemoFunction:*")
                       .Select("DemoFunction:*", environmentLabel)
                       // Configure to reload configuration if the registered 'Sentinel' key is modified
                       .ConfigureRefresh(refreshOptions =>
                            refreshOptions.Register(key: "DemoFunction:Sentinel", label: environmentLabel, refreshAll: true)
                                          .SetCacheExpiration(TimeSpan.FromSeconds(cacheExpiryInSeconds))
                       );
                ConfigurationRefresher = options.GetRefresher();
            });
            Configuration = ConfigurationBuilder.Build();

            builder.Services.AddLogging();
            builder.Services.AddSingleton(Configuration);
        }
    }
}
