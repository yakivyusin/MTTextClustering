using MTTextClustering.Methods.Contracts;
using MTTextClustering.Models;
using System;

namespace MTTextClustering.Factories
{
    public static class KClusteringMethodFactory
    {
        public static IKClusteringMethod Create(ClusteringMethods method) => method switch
        {
            _ => throw new NotImplementedException(),
        };
    }
}
