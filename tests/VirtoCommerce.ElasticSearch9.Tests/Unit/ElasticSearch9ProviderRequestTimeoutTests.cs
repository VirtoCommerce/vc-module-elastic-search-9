using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.ElasticSearch9.Core.Models;
using VirtoCommerce.ElasticSearch9.Core.Services;
using VirtoCommerce.ElasticSearch9.Data.Services;
using VirtoCommerce.Platform.Core.DistributedLock;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ElasticSearch9.Tests.Unit;

[Trait("Category", "Unit")]
public class ElasticSearch9ProviderRequestTimeoutTests
{
    [Fact]
    public void ElasticSearch9Options_Defaults_AreThirtySecondsAndTenMinutes()
    {
        var options = new ElasticSearch9Options();

        options.RequestTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.LongRunningRequestTimeout.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void Constructor_RequestTimeoutConfigured_BoundsTheClient()
    {
        var provider = new TestElasticSearch9Provider(new ElasticSearch9Options
        {
            Server = "http://localhost:9200",
            RequestTimeout = TimeSpan.FromSeconds(5),
        });

        provider.ClientRequestTimeout.Should().Be(TimeSpan.FromSeconds(5));
    }

    private sealed class TestElasticSearch9Provider(ElasticSearch9Options elasticOptions) : ElasticSearch9Provider(
        Options.Create(new SearchOptions { Scope = "test-core", Provider = "ElasticSearch9" }),
        Options.Create(elasticOptions),
        Mock.Of<ISettingsManager>(),
        Mock.Of<IElasticSearchRequestBuilder>(),
        Mock.Of<IElasticSearchResponseBuilder>(),
        Mock.Of<IElasticSearchDocumentConverter>(),
        Mock.Of<ILogger<ElasticSearch9Provider>>(),
        Mock.Of<IElasticSearchPropertyService>(),
        Mock.Of<IDistributedLockService>())
    {
        public TimeSpan? ClientRequestTimeout => Client.ElasticsearchClientSettings.RequestTimeout;
    }
}
