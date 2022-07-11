using MTTextClustering.Models;

namespace MTTextClustering.Methods.Contracts
{
    public interface IClusteringMethod
    {
        List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params);
    }
}
