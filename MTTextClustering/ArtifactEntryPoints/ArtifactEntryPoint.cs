using MTaaS.Attributes;
using MTTextClustering.Factories;
using MTTextClustering.Methods.Evaluation;
using MTTextClustering.Models;

namespace MTTextClustering.ArtifactEntryPoints
{
    [ArtifactEntryPoint("ReverseTexts")]
    public class ArtifactEntryPoint
    {
        private readonly DunnIndex _dunnIndex = new();
        private readonly DaviesBouldinIndex _daviesBouldinIndex = new();

        public OutputModel Launch(InputModel model)
        {
            var clusteringMethod = ClusteringMethodFactory.Create(model.Method);
            var clusters = clusteringMethod.Clusterize(model.Texts, model.Params);

            return new OutputModel(
                clusters,
                _dunnIndex.Evaluate(model.Texts, clusters),
                _daviesBouldinIndex.Evaluate(model.Texts, clusters));
        }
    }
}
