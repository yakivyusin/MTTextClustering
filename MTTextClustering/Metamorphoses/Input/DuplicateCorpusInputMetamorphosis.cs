using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("DuplicateCorpus")]
    public class DuplicateCorpusInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            var texts = model.Texts.ToList();
            var duplicates = texts
                .Select(x =>
                {
                    var id = x.Id.ToByteArray();
                    id[0] += 100;
                    return x with { Id = new Guid(id) };
                })
                .ToList();

            texts.AddRange(duplicates);

            return model with { Texts = texts.ToArray() };
        }
    }
}
