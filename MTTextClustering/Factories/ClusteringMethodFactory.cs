using MTTextClustering.Methods;
using MTTextClustering.Methods.Contracts;
using MTTextClustering.Models;
using System;

namespace MTTextClustering.Factories
{
    public static class ClusteringMethodFactory
    {
        public static IClusteringMethod Create(ClusteringMethods method) => method switch
        {
            ClusteringMethods.BisectingKMeans => new BisectingKMeans(),
            ClusteringMethods.SingleLink => new HierarchicalSingleLink(),
            ClusteringMethods.KMeans => new FarthestPointsKMeans(),
            _ => throw new NotImplementedException(),
        };
    }
}
