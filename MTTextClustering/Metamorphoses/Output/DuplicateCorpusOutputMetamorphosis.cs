using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("DuplicateCorpus")]
    public class DuplicateCorpusOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            var clusters = model.Clusters.Select(x => x.ToList()).ToList();

            foreach (var cluster in clusters)
            {
                var duplicates = cluster
                    .Select(x =>
                    {
                        var id = x.ToByteArray();
                        id[0] += 100;
                        return new Guid(id);
                    })
                    .ToList();

                cluster.AddRange(duplicates);
            }

            return model with { Clusters = clusters };
        }
    }
}
