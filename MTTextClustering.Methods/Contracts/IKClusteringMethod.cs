using MTTextClustering.Models;

namespace MTTextClustering.Methods.Contracts
{
    public interface IKClusteringMethod
    {
        List<List<Guid>> Clusterize(Text[] texts, int k, Dictionary<string, object> @params);
    }
}
