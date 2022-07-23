using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("ReverseParagraphTerms")]
    public class ReverseParagraphTermsInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            return model with
            {
                Texts = model
                    .Texts
                    .Select(x => ReverseParagraphTerms(x))
                    .ToArray()
            };
        }

        private Text ReverseParagraphTerms(Text text)
        {
            return text with
            {
                Content = string.Join(
                    Environment.NewLine,
                    text
                        .Content
                        .Split(Environment.NewLine)
                        .Select(x => string.Join(' ', x.Split(" ").Reverse())))
            };
        }
    }
}
