using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("DuplicateText")]
    public class DuplicateTextOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            var smallestTextId = model.Clusters.SelectMany(x => x).OrderBy(x => x).First();
            var clusterId = model.Clusters.FindIndex(x => x.Contains(smallestTextId));
            var resultClusters = model.Clusters.Select(x => x.ToList()).ToList();
            resultClusters[clusterId].Add(Guid.Empty);

            return model with { Clusters = resultClusters };
        }
    }
}
