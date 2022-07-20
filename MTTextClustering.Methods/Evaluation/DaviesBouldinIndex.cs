using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Evaluation
{
    public class DaviesBouldinIndex : MetricEvaluationBase
    {
        public override double Evaluate(Text[] texts, List<List<Guid>> clusters)
        {
            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();
            var sum = 0.0;

            for (int i = 0; i < clusters.Count; i++)
            {
                var maximumValue = Double.MinValue;

                for (int j = 0; j < clusters.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var intraDistance1 = GetIntraDistance(texts, clusters[i], tfIdf);
                    var intraDistance2 = GetIntraDistance(texts, clusters[j], tfIdf);
                    var interDistance = GetInterDistance(texts, clusters[i], clusters[j], tfIdf);

                    maximumValue = Math.Max(maximumValue, (intraDistance1 + intraDistance2) / interDistance);
                }

                sum += maximumValue;
            }

            return sum / clusters.Count;
        }
    }
}
