namespace MTTextClustering.Models
{
    public record InputModel(
        Text[] Texts,
        ClusteringMethods Method,
        Dictionary<string, object> Params)
    {
    }
}
