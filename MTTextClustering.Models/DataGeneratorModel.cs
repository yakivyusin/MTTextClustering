using MTTextClustering.Models.JsonConverters;
using System.Text.Json.Serialization;

namespace MTTextClustering.Models
{
    public record DataGeneratorModel(
        int TermsCount,
        ClusteringMethods Method,
        [property: JsonConverter(typeof(DictionaryStringObjectJsonConverter))] Dictionary<string, object> Params)
    {
    }
}
