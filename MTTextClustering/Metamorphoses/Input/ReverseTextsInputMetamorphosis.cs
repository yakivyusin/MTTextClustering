using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("ReverseTexts")]
    public class ReverseTextsInputMetamorphosis
    {
        public InputModel Trasform(InputModel model)
        {
            return model with { Texts = model.Texts.Reverse().ToArray() };
        }
    }
}
