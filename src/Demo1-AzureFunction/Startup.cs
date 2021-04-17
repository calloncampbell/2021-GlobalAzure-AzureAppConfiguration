using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

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
                                          //.SetCacheExpiration(TimeSpan.FromSeconds(cacheExpiryInSeconds))
                                          .SetCacheExpiration(TimeSpan.FromDays(30))                                          
                       )
                       // Indicate to load feature flags
                       .UseFeatureFlags(flagOptions =>
                       {
                           flagOptions.Label = environmentLabel;
                           //flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(cacheExpiryInSeconds);
                           flagOptions.CacheExpirationInterval = TimeSpan.FromDays(30);
                       });
                ConfigurationRefresher = options.GetRefresher();
            });

            // Load configuration from Azure App Configuration (Primary or Secondary Stores)
            var connectionString_PrimaryStore = Environment.GetEnvironmentVariable("AzureAppConfigConnectionString_PrimaryStore");
            var connectionString_SecondaryStore = Environment.GetEnvironmentVariable("AzureAppConfigConnectionString_SecondaryStore");
            var cacheExpiryInSeconds = double.Parse(Environment.GetEnvironmentVariable("AzureAppConfigurationCacheExpirationTimeInSeconds") ?? "900");
            var environmentLabel = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AzureAppConfigurationEnvironmentLabel"))
                ? Environment.GetEnvironmentVariable("AzureAppConfigurationEnvironmentLabel")
                : LabelFilter.Null;

            // Arrange your code so that it loads from the secondary store first and then the primary store.
            // This approach ensures that the configuration data in the primary store takes precedence whenever it's available.
            ConfigurationBuilder
                .AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString_SecondaryStore)
                           // Load all keys that start with `DemoFunction:`
                           .Select("DemoFunction:*")
                           .Select("DemoFunction:*", environmentLabel)
                           // Configure to reload configuration if the registered 'Sentinel' key is modified
                           .ConfigureRefresh(refreshOptions =>
                                refreshOptions.Register(key: "DemoFunction:Sentinel", label: environmentLabel, refreshAll: true)
                                          //.SetCacheExpiration(TimeSpan.FromSeconds(cacheExpiryInSeconds))
                                          .SetCacheExpiration(TimeSpan.FromDays(30))
                           )
                           // Indicate to load feature flags
                           .UseFeatureFlags(flagOptions =>
                           {
                               flagOptions.Label = environmentLabel;
                               flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(cacheExpiryInSeconds);
                           });
                    ConfigurationRefresher = options.GetRefresher();
                }, optional: true)
                .AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString_PrimaryStore)
                           // Load all keys that start with `DemoFunction:`
                           .Select("DemoFunction:*")
                           .Select("DemoFunction:*", environmentLabel)
                           // Configure to reload configuration if the registered 'Sentinel' key is modified
                           .ConfigureRefresh(refreshOptions =>
                                refreshOptions.Register(key: "DemoFunction:Sentinel", label: environmentLabel, refreshAll: true)
                                          //.SetCacheExpiration(TimeSpan.FromSeconds(cacheExpiryInSeconds))
                                          .SetCacheExpiration(TimeSpan.FromDays(30))
                           )
                           // Indicate to load feature flags
                           .UseFeatureFlags(flagOptions =>
                           {
                               flagOptions.Label = environmentLabel;
                               flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(cacheExpiryInSeconds);
                           });
                    ConfigurationRefresher = options.GetRefresher();
                }, optional: true);

            Configuration = ConfigurationBuilder.Build();

            builder.Services.AddLogging();
            builder.Services.AddSingleton(Configuration);
            builder.Services.AddSingleton(ConfigurationRefresher);
            builder.Services.AddFeatureManagement(Configuration);
        }
    }
}
