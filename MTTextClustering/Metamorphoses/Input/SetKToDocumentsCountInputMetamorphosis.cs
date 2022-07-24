using MTaaS.Attributes.Metamorphoses;
using MTTextClustering.Models;
using System.Linq;

namespace MTTextClustering.Metamorphoses.Input
{
    [InputMetamorphosis("SetKToDocumentsCount")]
    public class SetKToDocumentsCountInputMetamorphosis
    {
        public InputModel Transform(InputModel model)
        {
            var @params = model.Params.ToDictionary(x => x.Key, x => x.Value);
            @params["k"] = model.Texts.Length;

            return model with
            {
                Params = @params
            };
        }
    }
}
