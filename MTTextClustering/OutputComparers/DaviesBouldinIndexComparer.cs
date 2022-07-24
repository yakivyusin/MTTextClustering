using MTaaS.Attributes;
using MTTextClustering.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTTextClustering.OutputComparers
{
    public abstract class DaviesBouldinIndexComparer : IEqualityComparer<OutputModel>
    {
        private readonly Func<double, double, bool> _comparisonFunc;

        public DaviesBouldinIndexComparer(Func<double, double, bool> comparisonFunc)
        {
            _comparisonFunc = comparisonFunc;
        }

        public bool Equals(OutputModel x, OutputModel y)
        {
            return _comparisonFunc(x.DaviesBouldinIndex, y.DaviesBouldinIndex);
        }

        public int GetHashCode([DisallowNull] OutputModel obj)
        {
            return obj.DaviesBouldinIndex.GetHashCode();
        }
    }

    [OutputModelComparer("AddOutlineTextDBI")]
    public class DbiLessThanOrEqual : DaviesBouldinIndexComparer
    {
        public DbiLessThanOrEqual() : base((x, y) => y <= x)
        {

        }
    }
}
