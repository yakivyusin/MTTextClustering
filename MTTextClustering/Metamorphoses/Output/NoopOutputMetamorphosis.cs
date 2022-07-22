using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("ReverseTexts")]
    public class NoopOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            return model with { };
        }
    }
}
