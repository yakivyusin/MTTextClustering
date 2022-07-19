using MTTextClustering.Methods;
using MTTextClustering.Methods.Contracts;
using MTTextClustering.Models;
using System;

namespace MTTextClustering.Factories
{
    public static class KClusteringMethodFactory
    {
        public static IKClusteringMethod Create(ClusteringMethods method) => method switch
        {
            ClusteringMethods.BisectingKMeans => new BisectingKMeans(),
            ClusteringMethods.KMeans => new FarthestPointsKMeans(),
            ClusteringMethods.ModifiedIsland => new ModifiedIslandClustering(),
            _ => throw new NotImplementedException(),
        };
    }
}
