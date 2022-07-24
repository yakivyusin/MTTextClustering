using MTaaS.Attributes;
using MTTextClustering.Factories;
using MTTextClustering.Methods.Evaluation;
using MTTextClustering.Models;

namespace MTTextClustering.ArtifactEntryPoints
{
    [ArtifactEntryPoint("AddOutlineText")]
    [ArtifactEntryPoint("AddOutlineTextDBI")]
    [ArtifactEntryPoint("SetKToDocumentsCount")]
    [ArtifactEntryPoint("DecreaseCorpus")]
    public class KArtifactEntryPoint
    {
        private readonly DunnIndex _dunnIndex = new();
        private readonly DaviesBouldinIndex _daviesBouldinIndex = new();

        public OutputModel Launch(InputModel model)
        {
            var clusteringMethod = KClusteringMethodFactory.Create(model.Method);
            var clusters = clusteringMethod.Clusterize(model.Texts, (int)model.Params["k"], model.Params);

            return new OutputModel(
                clusters,
                _dunnIndex.Evaluate(model.Texts, clusters),
                _daviesBouldinIndex.Evaluate(model.Texts, clusters));
        }
    }
}
