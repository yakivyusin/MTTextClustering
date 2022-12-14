using MTaaS.Attributes;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MTTextClustering.OutputComparers
{
    [OutputModelComparer("ReverseTexts")]
    [OutputModelComparer("DuplicateText")]
    [OutputModelComparer("DuplicateCorpus")]
    [OutputModelComparer("ReverseTerms")]
    [OutputModelComparer("ReverseParagraphs")]
    [OutputModelComparer("DoubleTerms")]
    [OutputModelComparer("ReverseParagraphTerms")]
    [OutputModelComparer("SetKToDocumentsCount")]
    [OutputModelComparer("DecreaseCorpus")]
    public class ClustersIgnoreOrderComparer : IEqualityComparer<OutputModel>
    {
        public bool Equals(OutputModel x, OutputModel y)
        {
            var xClusters = x.Clusters;
            var yClusters = y.Clusters;

            if (xClusters.Count != yClusters.Count)
            {
                return false;
            }

            return
                xClusters.All(x => yClusters.Any(y => new HashSet<Guid>(x).SetEquals(new HashSet<Guid>(y)))) &&
                yClusters.All(y => xClusters.Any(x => new HashSet<Guid>(y).SetEquals(new HashSet<Guid>(x))));
        }

        public int GetHashCode([DisallowNull] OutputModel obj)
        {
            return obj.GetHashCode();
        }
    }
}
