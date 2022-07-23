using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("ReverseTerms")]
    public class ReverseTermsInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            return model with
            {
                Texts = model
                    .Texts
                    .Select(x => x with { Content = string.Join(' ', x.Terms.Reverse()) })
                    .ToArray()
            };
        }
    }
}
