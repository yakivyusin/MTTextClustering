using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("DecreaseCorpus")]
    public class DecreaseCorpusOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            var ids = model.Clusters
                .SelectMany(x => x)
                .OrderByDescending(x => x)
                .Take(model.Clusters.Count);

            return model with
            {
                Clusters = model
                    .Clusters
                    .SelectMany(x => x)
                    .OrderByDescending(x => x)
                    .Take(model.Clusters.Count)
                    .Select(x => new List<Guid> { x })
                    .ToList()
            };
        }
    }
}
