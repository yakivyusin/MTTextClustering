using MTTextClustering.Methods.Contracts;
using MTTextClustering.Models;
using System;

namespace MTTextClustering.Factories
{
    public static class ClusteringMethodFactory
    {
        public static IClusteringMethod Create(ClusteringMethods method) => method switch
        {
            _ => throw new NotImplementedException(),
        };
    }
}
