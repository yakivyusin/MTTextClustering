using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("ReverseParagraphs")]
    public class ReverseParagraphsInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            return model with
            {
                Texts = model
                    .Texts
                    .Select(x => x with { Content = string.Join(Environment.NewLine, x.Content.Split(Environment.NewLine).Reverse()) })
                    .ToArray()
            };
        }
    }
}
