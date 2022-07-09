namespace MTTextClustering.Models
{
    public record OutputModel(
        List<List<Guid>> Clusters,
        double DunnIndex,
        double DaviesBouldinIndex)
    {
    }
}
