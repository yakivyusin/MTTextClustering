using MTTextClustering.Models;

namespace MTTextClustering.Methods.Helpers
{
    internal class TfIdfCalculator
    {
        private readonly Text[] _corpus;
        private readonly string[] _uniqueLemmas;
        private readonly double[] _idfVector;

        public TfIdfCalculator(Text[] corpus)
        {
            _corpus = corpus;
            _uniqueLemmas = corpus
                .SelectMany(x => x.Terms)
                .Distinct()
                .ToArray();

            _idfVector = _uniqueLemmas
                .Select(x => Math.Log((double)_corpus.Length / _corpus.Count(t => t.Terms.Contains(x))))
                .ToArray();
        }

        public double[][] GetTfIdfMatrix()
        {
            var result = new double[_corpus.Length][];

            for (int i = 0; i < _corpus.Length; i++)
            {
                result[i] = new double[_uniqueLemmas.Length];
            }

            for (int i = 0; i < _corpus.Length; i++)
            {
                for (int j = 0; j < _uniqueLemmas.Length; j++)
                {
                    var tf = (double)_corpus[i].Terms.Count(x => x == _uniqueLemmas[j]) / _corpus[i].Terms.Count();

                    result[i][j] = tf * _idfVector[j];
                }
            }

            return result;
        }
    }
}
