using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VirtoCommerce.ElasticSearch9.Tests.Integration;

namespace VirtoCommerce.ElasticSearch9.Tests;

public class Startup
{
    public static void ConfigureHost(IHostBuilder hostBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ElasticSearch9Tests>()
            .AddEnvironmentVariables()
            .Build();

        hostBuilder.ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration));
    }
}
