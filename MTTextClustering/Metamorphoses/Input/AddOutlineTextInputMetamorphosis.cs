using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Globalization;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("AddOutlineText")]
    [InputMetamorphosis("AddOutlineTextDBI")]
    public class AddOutlineTextInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            var texts = model.Texts.ToList();
            var uniqueTerms = texts.SelectMany(x => x.Terms).Distinct().ToList();
            var maximumIndex = uniqueTerms
                .Select(x => int.Parse(x, NumberStyles.HexNumber))
                .OrderByDescending(x => x)
                .First();

            var newContent = string.Join(
                ' ',
                Enumerable
                    .Range(maximumIndex + 1, uniqueTerms.Count / texts.Count)
                    .SelectMany(x => Enumerable.Repeat(x, 5))
                    .Select(x => Convert.ToString(x, 16)));

            texts.Add(new Text(Guid.Empty, newContent));

            var @params = model.Params.ToDictionary(x => x.Key, x => x.Value);
            @params["k"] = 1 + (int)@params["k"];

            return model with { Texts = texts.ToArray(), Params = @params };
        }
    }
}
