using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Demo1_AzureFunction.Startup))]

namespace Demo1_AzureFunction
{
    public class Startup : FunctionsStartup
    {
        private static IConfigurationRoot Configuration { get; set; }
        public IConfigurationBuilder ConfigurationBuilder { get; set; }

        public Startup()
        {
            ConfigurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables();

            Configuration = ConfigurationBuilder.Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton(Configuration);
        }
    }
}
