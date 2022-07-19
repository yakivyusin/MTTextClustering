using MTTextClustering.Models;

namespace MTTextClustering.Methods.Contracts
{
    public interface IEvaluationMethod
    {
        double Evaluate(Text[] texts, List<List<Guid>> clusters);
    }
}
