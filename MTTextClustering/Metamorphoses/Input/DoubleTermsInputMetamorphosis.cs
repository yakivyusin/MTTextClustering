using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("DoubleTerms")]
    public class DoubleTermsInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            return model with
            {
                Texts = model
                    .Texts
                    .Select(x => x with { Content = string.Concat(x.Content, Environment.NewLine, x.Content) })
                    .ToArray()
            };
        }
    }
}
