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
            ClusteringMethods.KMeans => new FarthestPointsKMeans(),
            _ => throw new NotImplementedException(),
        };
    }
}
