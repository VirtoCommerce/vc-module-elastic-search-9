using Elastic.Clients.Elasticsearch.Mapping;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ElasticSearch9.Core.Services;

public interface IElasticSearchPropertyService
{
    IProperty CreateProperty(IndexDocumentField field);
    void ConfigureProperty(IProperty property, IndexDocumentField field);
}
