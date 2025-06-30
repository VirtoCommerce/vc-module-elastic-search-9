using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.ElasticSearch9.Core.Models;
using VirtoCommerce.ElasticSearch9.Data.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using Xunit;

namespace VirtoCommerce.ElasticSearch9.Tests.Integration;

[Trait("Category", "CI")]
[Trait("Category", "IntegrationTest")]
public class ElasticSearch9Tests : SearchProviderTests
{
    private readonly IConfiguration _configuration;

    public ElasticSearch9Tests(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override ISearchProvider GetSearchProvider()
    {
        var searchOptions = Options.Create(new SearchOptions { Scope = "test-core", Provider = "ElasticSearch9" });
        var elasticOptions = Options.Create(_configuration.GetSection("ElasticSearch9").Get<ElasticSearch9Options>());
        elasticOptions.Value.Server ??= Environment.GetEnvironmentVariable("TestElasticsearchHost") ?? "localhost:9200";

        var settingsManager = GetSettingsManager();

        var loggerFactory = LoggerFactory.Create(builder => { builder.ClearProviders(); });

        var filtersBuilder = new ElasticSearchFiltersBuilder();
        var aggregationsBuilder = new ElasticSearchAggregationsBuilder(filtersBuilder);
        var builderLogger = loggerFactory.CreateLogger<ElasticSearchRequestBuilder>();
        var requestBuilder = new ElasticSearchRequestBuilder(filtersBuilder, aggregationsBuilder, settingsManager, builderLogger);

        var responseBuilder = new ElasticSearchResponseBuilder();
        var propertyService = new ElasticSearchPropertyService();

        var providerLogger = loggerFactory.CreateLogger<ElasticSearch9Provider>();

        var provider = new ElasticSearch9Provider(
            searchOptions,
            elasticOptions,
            settingsManager,
            requestBuilder,
            responseBuilder,
            propertyService,
            providerLogger
            );

        return provider;
    }
}
