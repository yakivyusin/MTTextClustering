using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("AddOutlineText")]
    public class AddOutlineTextOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            var clusters = model.Clusters.Select(x => x.ToList()).ToList();
            clusters.Add(new List<Guid> { Guid.Empty });

            return model with { Clusters = clusters };
        }
    }
}
