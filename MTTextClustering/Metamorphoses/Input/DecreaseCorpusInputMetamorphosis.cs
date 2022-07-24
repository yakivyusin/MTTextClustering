using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("DecreaseCorpus")]
    public class DecreaseCorpusInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            return model with
            {
                Texts = model.Texts.Take((int)model.Params["k"]).ToArray()
            };
        }
    }
}
