using MTTextClustering.Methods.Contracts;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Evaluation
{
    public abstract class MetricEvaluationBase : IEvaluationMethod
    {
        public abstract double Evaluate(Text[] texts, List<List<Guid>> clusters);

        protected double GetIntraDistance(Text[] texts, List<Guid> cluster, double[][] tfIdf)
        {
            var clusterVectors = texts
                .Where(x => cluster.Contains(x.Id))
                .Select(x => Array.IndexOf(texts, x))
                .Select(x => tfIdf[x])
                .ToArray();

            var maximumPairDistance = 0.0;

            for (int i = 0; i < clusterVectors.Length; i++)
            {
                for (int j = i + 1; j < clusterVectors.Length; j++)
                {
                    var distance = GetVectorDistance(clusterVectors[i], clusterVectors[j]);

                    maximumPairDistance = Math.Max(maximumPairDistance, distance);
                }
            }

            return maximumPairDistance;
        }

        protected double GetInterDistance(Text[] texts, List<Guid> cluster1, List<Guid> cluster2, double[][] tfIdf)
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
                    var distance = GetVectorDistance(vector, otherVector);

                    minDistance = Math.Min(minDistance, distance);
                }
            }

            return minDistance;
        }

        private double GetVectorDistance(double[] vector1, double[] vector2)
        {
            var distance = 0.0;

            for (int i = 0; i < vector1.Length; i++)
            {
                distance += Math.Pow(vector1[i] - vector2[i], 2);
            }

            return Math.Sqrt(distance);
        }
    }
}
