using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
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
public class ElasticSearch9ProviderCreateIndexLockTests
{
    [Fact]
    public async Task InternalCreateIndexWithLockAsync_PreventsDuplicateIndexCreation()
    {
        // Arrange
        var indexStoreWithoutLock = new IndexStore(new Barrier(2));
        var providerWithoutLock = new TestElasticSearch9Provider(indexStoreWithoutLock);

        // Act
        await Task.WhenAll(
            providerWithoutLock.CallInternalCreateIndexAsync(),
            providerWithoutLock.CallInternalCreateIndexAsync());

        // Assert
        indexStoreWithoutLock.CreatedIndexCount.Should().Be(2, "without a lock both calls pass the index existence check before the index is created");

        var indexStoreWithLock = new IndexStore();
        var providerWithLock = new TestElasticSearch9Provider(indexStoreWithLock);

        await Task.WhenAll(
            providerWithLock.CallInternalCreateIndexWithLockAsync(),
            providerWithLock.CallInternalCreateIndexWithLockAsync());

        indexStoreWithLock.CreatedIndexCount.Should().Be(1, "the lock should serialize index creation and prevent duplicate indexes");
    }

    private sealed class TestElasticSearch9Provider(IndexStore indexStore) : ElasticSearch9Provider(
        Options.Create(new SearchOptions { Scope = "test-core", Provider = "ElasticSearch9" }),
        Options.Create(new ElasticSearch9Options()),
        Mock.Of<ISettingsManager>(),
        Mock.Of<IElasticSearchRequestBuilder>(),
        Mock.Of<IElasticSearchResponseBuilder>(),
        Mock.Of<IElasticSearchDocumentConverter>(),
        Mock.Of<ILogger<ElasticSearch9Provider>>(),
        Mock.Of<IElasticSearchPropertyService>(),
        new PassThroughDistributedLockService())
    {
        private const string DocumentType = "Product";

        public Task CallInternalCreateIndexAsync() => InternalCreateIndexAsync(DocumentType, [], new IndexingParameters());

        public Task CallInternalCreateIndexWithLockAsync() => InternalCreateIndexWithLockAsync(DocumentType, [], new IndexingParameters());

        protected override Task<IDictionary<PropertyName, IProperty>> GetMappingAsync(string indexName) => Task.FromResult<IDictionary<PropertyName, IProperty>>(new Dictionary<PropertyName, IProperty>());

        protected override Task<bool> IndexExistsAsync(string indexName) => indexStore.IndexExistsAsync();

        protected override Task CreateIndexAsync(string documentType, string indexName, string alias)
        {
            indexStore.CreateIndex();

            return Task.CompletedTask;
        }

        protected override Task UpdateMappingAsync(string documentType, string indexName, Properties properties) => Task.CompletedTask;
    }

    private sealed class IndexStore(Barrier indexExistsBarrier = null)
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        private int _createdIndexCount;

        public int CreatedIndexCount => Volatile.Read(ref _createdIndexCount);

        public async Task<bool> IndexExistsAsync()
        {
            var indexExists = CreatedIndexCount > 0;
            await Task.Yield();
            indexExistsBarrier?.SignalAndWait(Timeout);

            return indexExists;
        }

        public void CreateIndex() => Interlocked.Increment(ref _createdIndexCount);
    }

    private sealed class PassThroughDistributedLockService : IDistributedLockService
    {
        public T Execute<T>(string resourceKey, Func<T> resolver, TimeSpan? lockTimeout = null, TimeSpan? tryLockTimeout = null, TimeSpan? retryInterval = null, CancellationToken? cancellationToken = null) => resolver();

        public Task<T> ExecuteAsync<T>(string resourceKey, Func<Task<T>> resolver, TimeSpan? lockTimeout = null, TimeSpan? tryLockTimeout = null, TimeSpan? retryInterval = null, CancellationToken? cancellationToken = null) => resolver();
    }
}
