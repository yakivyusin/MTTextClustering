namespace MTTextClustering.Models
{
    public record DataGeneratorModel(
        int TermsCount,
        ClusteringMethods Method,
        Dictionary<string, object> Params)
    {
    }
}
