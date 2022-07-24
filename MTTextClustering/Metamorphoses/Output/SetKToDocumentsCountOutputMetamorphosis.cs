using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("SetKToDocumentsCount")]
    public class SetKToDocumentsCountOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            var ids = model.Clusters.SelectMany(x => x).Distinct().ToList();

            return model with
            {
                Clusters = ids.Select(x => new List<Guid> { x }).ToList()
            };
        }
    }
}
