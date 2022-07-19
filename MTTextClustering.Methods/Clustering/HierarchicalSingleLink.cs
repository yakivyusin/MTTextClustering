using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Clustering
{
    // https://github.com/chen0040/java-clustering/blob/master/src/main/java/com/github/chen0040/clustering/hierarchical/HierarchicalClustering.java
    public class HierarchicalSingleLink : IClusteringMethod
    {
        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            var threshold = (double)@params["threshold"];
            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();
            var clusters = texts.Select((t, i) => new Cluster(i, tfIdf[i], t.Id)).ToArray();

            while (true)
            {
                var select_i = -1;
                var select_j = -1;
                var minDistance = double.MaxValue;

                for (int i = 0; i < clusters.Length; i++)
                {
                    for (int j = i + 1; j < clusters.Length; j++)
                    {
                        var distance = clusters[i].GetSingleLinkDistance(clusters[j]);

                        if (distance < minDistance)
                        {
                            select_i = i;
                            select_j = j;
                            minDistance = distance;
                        }
                    }
                }

                if (minDistance >= threshold)
                {
                    break;
                }

                var newClusters = new Cluster[clusters.Length - 1];

                int newIndex = 0;
                for (int i = 0; i < clusters.Length; i++)
                {
                    if (i != select_i && i != select_j)
                    {
                        newClusters[newIndex++] = clusters[i];
                    }
                }

                clusters[select_i].Add(clusters[select_j]);
                newClusters[newIndex] = clusters[select_i];

                clusters = newClusters;
            }

            return clusters
                .Select(x => x.Guids)
                .ToList();
        }

        private class Cluster
        {
            public int Index { get; set; }

            public List<double[]> Vectors { get; set; }

            public List<Guid> Guids { get; set; }

            public Cluster(int index, double[] vector, Guid guid)
            {
                Index = index;
                Vectors = new List<double[]> { vector };
                Guids = new List<Guid> { guid };
            }

            public double GetSingleLinkDistance(Cluster other)
            {
                var minDistance = double.MaxValue;

                foreach (var vector in Vectors)
                {
                    foreach (var otherVector in other.Vectors)
                    {
                        var distance = 0.0;

                        for (int i = 0; i < vector.Length; i++)
                        {
                            distance += Math.Pow(vector[i] - otherVector[i], 2);
                        }

                        distance = Math.Sqrt(distance);

                        minDistance = Math.Min(minDistance, distance);
                    }
                }

                return minDistance;
            }

            public void Add(Cluster other)
            {
                Vectors.AddRange(other.Vectors);
                Guids.AddRange(other.Guids);

                other.Vectors.Clear();
                other.Guids.Clear();
            }
        }
    }
}
