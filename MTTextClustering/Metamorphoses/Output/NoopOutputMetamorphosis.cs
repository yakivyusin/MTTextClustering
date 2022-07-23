using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;

namespace MTTextClustering.Metamorphoses.Output
{
    [OutputMetamorphosis("ReverseTexts")]
    [OutputMetamorphosis("ReverseTerms")]
    [OutputMetamorphosis("ReverseParagraphs")]
    [OutputMetamorphosis("DoubleTerms")]
    [OutputMetamorphosis("ReverseParagraphTerms")]
    public class NoopOutputMetamorphosis
    {
        public OutputModel Transform(OutputModel model)
        {
            return model with { };
        }
    }
}
