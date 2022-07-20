using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Evaluation
{
    public class DunnIndex : MetricEvaluationBase
    {
        public override double Evaluate(Text[] texts, List<List<Guid>> clusters)
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
    }
}
