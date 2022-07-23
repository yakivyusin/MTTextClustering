using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("DuplicateText")]
    public class DuplicateTextInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            var textWithSmallestId = model.Texts.OrderBy(x => x.Id).First();
            var textsWithCopy = model.Texts.ToList();
            textsWithCopy.Add(textWithSmallestId with { Id = Guid.Empty });

            return model with { Texts = textsWithCopy.ToArray() };
        }
    }
}
