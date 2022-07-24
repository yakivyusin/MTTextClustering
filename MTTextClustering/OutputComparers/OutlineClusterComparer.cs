using MTaaS.Attributes;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MTTextClustering.OutputComparers
{
    [OutputModelComparer("AddOutlineText")]
    public class OutlineClusterComparer : IEqualityComparer<OutputModel>
    {
        public bool Equals(OutputModel x, OutputModel y)
        {
            return
                x.Clusters.Any(x => x.Count == 1 && x[0] == Guid.Empty) &&
                y.Clusters.Any(y => y.Count == 1 && y[0] == Guid.Empty);
        }

        public int GetHashCode([DisallowNull] OutputModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
