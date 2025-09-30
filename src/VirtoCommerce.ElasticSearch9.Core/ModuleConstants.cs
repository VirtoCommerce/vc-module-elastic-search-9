using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ElasticSearch9.Core;

public static class ModuleConstants
{
    public const string ProviderName = "ElasticSearch9";
    public const string ElasticSearchExceptionTitle = "Elasticsearch9 Server";

    public const string ScoreFieldName = "score";
    // For compatibility with Elastic App Search
    public const string ElasticScoreFieldName = "_score";

    public const string ActiveIndexAlias = "active";
    public const string BackupIndexAlias = "backup";
    public const string SearchableFieldAnalyzerName = "searchable_field_analyzer";
    public const string NGramFilterName = "custom_ngram";
    public const string EdgeNGramFilterName = "custom_edge_ngram";
    public const string CompletionSubFieldName = "completion";
    public const int SuggestionFieldLength = 256;

    // Semantic/vector search section
    public const string NoModel = "Disabled";
    public const string ElserModel = "ELSER";
    public const string ThirdPartyModel = "ThirdParty";

    public const string ModelPropertyName = "__ml";
    public const string TokensFieldName = "tokens";
    public const string VectorFieldName = "predicted_value";

    public const string TokensPropertyName = $"{ModelPropertyName}.{TokensFieldName}";
    public const string VectorPropertyName = $"{ModelPropertyName}.{VectorFieldName}";

    public const int DefaultIndexTotalFieldsLimit = 1000;
    public const int DefaultMinGram = 1;
    public const int DefaultMaxGram = 50;
    public const int DefaultSemanticVectorModelDimensions = 384;
    public const decimal DefaultMinScore = 0.1M;
    public const decimal DefaultBoost = 1.0M;

    public static class Settings
    {
        public static class General
        {


            public static SettingDescriptor IndexTotalFieldsLimit { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.IndexTotalFieldsLimit",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.Integer,
                DefaultValue = DefaultIndexTotalFieldsLimit,
            };

            public static SettingDescriptor TokenFilter { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.TokenFilter",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.ShortText,
                DefaultValue = "custom_edge_ngram",
            };

            public static SettingDescriptor MinGram { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.NGramTokenFilter.MinGram",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.Integer,
                DefaultValue = DefaultMinGram,
            };

            public static SettingDescriptor MaxGram { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.NGramTokenFilter.MaxGram",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.Integer,
                DefaultValue = DefaultMaxGram,
            };

            public static SettingDescriptor MinScore { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.MinScore",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.Decimal,
                DefaultValue = DefaultMinScore,
            };

            public static SettingDescriptor MinScorePerDocumentType { get; } = new SettingDescriptor
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.MinScorePerDocumentType",
                GroupName = "Search|ElasticSearch9|General",
                ValueType = SettingValueType.Json,
                DefaultValue =
                    $$"""
                    [
                        {
                          "documentType": "Product",
                          "minScore": 0.1
                        },
                        {
                          "documentType": "Category",
                          "minScore": 0.1
                        },
                        {
                          "documentType": "Member",
                          "minScore": 0.1
                        }
                    ]
                    """,

            };

            public static SettingDescriptor SemanticSearchType { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.SemanticSearchType",
                ValueType = SettingValueType.ShortText,
                GroupName = "Search|ElasticSearch9|Semantic",
                DefaultValue = NoModel,
                AllowedValues = [NoModel, ElserModel, ThirdPartyModel],
            };

            public static SettingDescriptor SemanticModelId { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.SemanticModelId",
                GroupName = "Search|ElasticSearch9|Semantic",
                ValueType = SettingValueType.ShortText,
                DefaultValue = ".elser_model_1",
            };

            public static SettingDescriptor SemanticPipelineName { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.SemanticPipelineName",
                GroupName = "Search|ElasticSearch9|Semantic",
                ValueType = SettingValueType.ShortText,
                DefaultValue = "elser-v1-pipeline",
            };

            public static SettingDescriptor SemanticVectorModelDimensions { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.SemanticVectorModelDimensions",
                GroupName = "Search|ElasticSearch9|Semantic",
                ValueType = SettingValueType.PositiveInteger,
                DefaultValue = DefaultSemanticVectorModelDimensions,
            };

            public static SettingDescriptor SemanticBoost { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.SemanticBoost",
                GroupName = "Search|ElasticSearch9|Semantic",
                ValueType = SettingValueType.Decimal,
                DefaultValue = DefaultBoost,
            };

            public static SettingDescriptor KeywordBoost { get; } = new()
            {
                Name = "VirtoCommerce.Search.ElasticSearch9.KeywordBoost",
                GroupName = "Search|ElasticSearch9|Semantic",
                ValueType = SettingValueType.Decimal,
                DefaultValue = DefaultBoost,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return IndexTotalFieldsLimit;
                    yield return TokenFilter;
                    yield return MinGram;
                    yield return MaxGram;
                    yield return MinScore;
                    yield return MinScorePerDocumentType;
                    yield return SemanticSearchType;
                    yield return SemanticModelId;
                    yield return SemanticPipelineName;
                    yield return SemanticVectorModelDimensions;
                    yield return SemanticBoost;
                    yield return KeywordBoost;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }
}
