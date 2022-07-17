using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods
{
    // https://github.com/sabbirahmad/BisectingKMeans/blob/master/src/BisectingKMeans.java
    public class BisectingKMeans : IClusteringMethod, IKClusteringMethod
    {
        public List<List<Guid>> Clusterize(Text[] texts, int k, Dictionary<string, object> @params)
        {
            var textsDic = texts.ToDictionary(x => x.Id);
            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();
            var clusters = new PriorityQueue<Cluster, double>(new ClusterComparer());
            var initialCluster = new Cluster(tfIdf, textsDic.Keys.ToArray());
            clusters.Enqueue(initialCluster, initialCluster.SE);

            for (int i = 1; i < k; i++)
            {
                var cluster = clusters.Dequeue();
                var textsToCluster = cluster.Guids.Select(x => textsDic[x]).ToArray();
                var clusterizedTexts = new FarthestPointsKMeans().Clusterize(textsToCluster, 2, @params);

                foreach (var clusterized in clusterizedTexts)
                {
                    var vectors = clusterized
                        .Select(x => Array.FindIndex(texts, y => y.Id == x))
                        .Select(ind => tfIdf[ind])
                        .ToArray();

                    var newCluster = new Cluster(vectors, clusterized.ToArray());
                    clusters.Enqueue(newCluster, newCluster.SE);
                }
            }

            return clusters
                .UnorderedItems
                .Select(x => x.Element.Guids.ToList())
                .ToList();
        }

        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            return Clusterize(texts, (int)@params["k"], @params);
        }

        private class Cluster
        {
            public double[][] Vectors { get; }

            public Guid[] Guids { get; }

            public double[] Centroid { get; }

            public double SE { get; }

            public Cluster(double[][] vectors, Guid[] guids)
            {
                Vectors = vectors;
                Guids = guids;
                Centroid = CalculateCentroid();
                SE = CalculateSE();
            }

            private double[] CalculateCentroid()
            {
                var centroid = new double[Vectors[0].Length];

                for (int i = 0; i < Vectors.Length; i++)
                {
                    for (int j = 0; j < Vectors[i].Length; j++)
                    {
                        centroid[j] += Vectors[i][j];
                    }
                }

                for (int i = 0; i < centroid.Length; i++)
                {
                    centroid[i] /= Vectors.Length;
                }

                return centroid;
            }

            private double CalculateSE()
            {
                var error = 0.0;

                for (int i = 0; i < Vectors.Length; i++)
                {
                    for (int j = 0; j < Vectors[i].Length; j++)
                    {
                        error += Math.Pow(Vectors[i][j] - Centroid[j], 2);
                    }
                }

                return error;
            }
        }

        private class ClusterComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                if ((y - x) > 0)
                    return 1;
                if ((y - x) < 0)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
