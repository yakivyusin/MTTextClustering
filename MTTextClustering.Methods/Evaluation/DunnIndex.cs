using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Evaluation
{
    public class DunnIndex : IEvaluationMethod
    {
        public double Evaluate(Text[] texts, List<List<Guid>> clusters)
        {
            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();

            var minimumInterDistance = Double.MaxValue;
            var maximumIntraDistance = Double.MinValue;

            for (int i = 0; i < clusters.Count; i++)
            {
                maximumIntraDistance = Math.Max(maximumIntraDistance, GetIntraDistance(texts, clusters[i], tfIdf));

                for (int j = i + 1; j < clusters.Count; j++)
                {
                    minimumInterDistance = Math.Min(minimumInterDistance, GetInterDistance(texts, clusters[i], clusters[j], tfIdf));
                }
            }

            return minimumInterDistance / maximumIntraDistance;
        }

        private double GetIntraDistance(Text[] texts, List<Guid> cluster, double[][] tfIdf)
        {
            var clusterTexts = texts.Where(x => cluster.Contains(x.Id)).ToArray();
            var clusterVectors = clusterTexts
                .Select(x => Array.IndexOf(texts, x))
                .Select(x => tfIdf[x])
                .ToArray();

            var maximumPairDistance = Double.MinValue;

            for (int i = 0; i < clusterVectors.Length; i++)
            {
                for (int j = i + 1; j < clusterVectors.Length; j++)
                {
                    var distance = 0.0;

                    for (int k = 0; k < clusterVectors[i].Length; k++)
                    {
                        distance += Math.Pow(clusterVectors[i][k] - clusterVectors[j][k], 2);
                    }

                    distance = Math.Sqrt(distance);
                    maximumPairDistance = Math.Max(maximumPairDistance, distance);
                }
            }

            return maximumPairDistance;
        }

        private double GetInterDistance(Text[] texts, List<Guid> cluster1, List<Guid> cluster2, double[][] tfIdf)
        {
            var cluster1Vectors = texts
                .Where(x => cluster1.Contains(x.Id))
                .Select(x => Array.IndexOf(texts, x))
                .Select(x => tfIdf[x])
                .ToArray();

            var cluster2Vectors = texts
                .Where(x => cluster2.Contains(x.Id))
                .Select(x => Array.IndexOf(texts, x))
                .Select(x => tfIdf[x])
                .ToArray();

            var minDistance = double.MaxValue;

            foreach (var vector in cluster1Vectors)
            {
                foreach (var otherVector in cluster2Vectors)
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
    }
}
