using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;
using System.Collections;

namespace MTTextClustering.Methods
{
    public class ModifiedIslandClustering : IClusteringMethod, IKClusteringMethod
    {
        private string[] allLemmas;
        private string[] uniqueLemmas;

        public List<List<Guid>> Clusterize(Text[] texts, int k, Dictionary<string, object> @params)
        {
            allLemmas = texts.SelectMany(x => x.Terms).ToArray();
            uniqueLemmas = allLemmas.Distinct().ToArray();

            var matrix = new MatrixCalculator().Matrix(texts, allLemmas, uniqueLemmas);
            var termClusters = MatrixClustering(matrix, texts, k);
            var textClusters = MapClusters(termClusters, texts);

            return textClusters
                .Select(x => x.Select(y => y.Id).ToList())
                .ToList();
        }

        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            return Clusterize(texts, (int)@params["k"], @params);
        }

        private List<Cluster<string>> MatrixClustering(Matrix matrix, Text[] texts, int k)
        {
            var filteredMatrix = matrix.ToList();

            var indexesList = filteredMatrix.
                OrderBy(x => x.Correlation).
                SelectMany(x => new int[] { x.Row, x.Column }).
                Distinct().
                ToList();
            var medoids = InitialMedoids(indexesList, matrix, k);
            var minCost = medoids.SelectMany(x => x.Points.Select(y => matrix[indexesList[x.Center], indexesList[y]])).Sum();
            List<Medoid> best = null;
            do
            {
                foreach (var cluster in medoids)
                {
                    foreach (var point in cluster.Points)
                    {
                        var temp = medoids.Where(x => x.Center != cluster.Center)
                            .Select(x => new Medoid() { Center = x.Center }).ToList();
                        temp.Add(new Medoid() { Center = point });
                        foreach (var i in Enumerable
                                            .Range(0, indexesList.Count)
                                            .Except(temp.Select(x => x.Center)))
                        {
                            temp.MinBy(x => matrix[indexesList[x.Center], indexesList[i]]).Points.Add(i);
                        }
                        var cost = temp.SelectMany(x => x.Points.Select(y => matrix[indexesList[x.Center], indexesList[y]])).Sum();
                        if (cost < minCost)
                        {
                            minCost = cost;
                            best = temp;
                        }
                    }
                }
                if (best != null)
                {
                    medoids = best;
                    best = null;
                }
                else
                {
                    break;
                }
            }
            while (true);

            return MapMedoids(indexesList, medoids);
        }

        private List<Medoid> InitialMedoids(List<int> indexesList, Matrix matrix, int k)
        {
            var clusters = new List<Medoid>();
            var maximumDistance = double.MinValue;

            foreach (var index in indexesList)
            {
                var farthestIndexes = indexesList
                    .Where(x => x != index)
                    .Select(x => new { X = x, Distance = matrix[index, x] })
                    .OrderByDescending(x => x.Distance)
                    .Take(k - 1)
                    .ToList();

                if (farthestIndexes.Sum(x => x.Distance) > maximumDistance)
                {
                    clusters = farthestIndexes
                        .Select(x => x.X)
                        .Union(new[] { index })
                        .Select(x => new Medoid { Center = indexesList.IndexOf(x) })
                        .ToList();

                    maximumDistance = farthestIndexes.Sum(x => x.Distance);
                }
            }

            foreach (var i in Enumerable
                .Range(0, indexesList.Count).Except(clusters.Select(x => x.Center)))
            {
                clusters.MinBy(x => matrix[indexesList[x.Center], indexesList[i]]).Points.Add(i);
            }
            return clusters;
        }

        private List<Cluster<string>> MapMedoids(List<int> indexesList, List<Medoid> medoids)
        {
            var termsClusters = new List<Cluster<string>>();
            foreach (var cluster in medoids)
            {
                var termsCluster = new Cluster<string>();
                termsCluster.Add(uniqueLemmas[indexesList[cluster.Center]]);
                foreach (var point in cluster.Points)
                {
                    termsCluster.Add(uniqueLemmas[indexesList[point]]);
                }
                termsClusters.Add(termsCluster);
            }
            return termsClusters;
        }

        private List<Cluster<Text>> MapClusters(List<Cluster<string>> termClusters, Text[] texts)
        {
            List<Cluster<Text>> clusters = Enumerable
                .Range(0, termClusters.Count)
                .Select(x => new Cluster<Text>()).ToList();

            foreach (var text in texts)
            {
                var counts = termClusters.Select(x => text.Terms.Select(l => x.Count(t => t == l)).Sum());
                clusters[counts.MaxIndex()].Add(text);
            }

            return clusters;
        }

        private class TermsIsland
        {
            public List<int> Terms { get; set; } = new List<int>();
            public List<MatrixLink> Links { get; set; } = new List<MatrixLink>();
        }

        private class MatrixLink
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public double Correlation { get; set; }
        }

        private class Matrix : IEnumerable<MatrixLink>
        {
            private int n;
            private double[] vector;

            public Matrix(int dimension)
            {
                n = dimension;
                vector = new double[n * (n - 1) / 2];
            }

            public int MatrixDimension
            {
                get
                {
                    return n;
                }
            }

            public int ElementsCount
            {
                get
                {
                    return vector.Length;
                }
            }

            public double this[int i, int j]
            {
                get
                {
                    return vector[GetVectorPosition(i, j)];
                }
                set
                {
                    vector[GetVectorPosition(i, j)] = value;
                }
            }

            public double this[int i]
            {
                get
                {
                    return vector[i];
                }
                set
                {
                    vector[i] = value;
                }
            }

            public IEnumerator<MatrixLink> GetEnumerator()
            {
                int vectorPos = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        yield return new MatrixLink { Row = i, Column = j, Correlation = vector[vectorPos] };
                        vectorPos++;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private int GetVectorPosition(int i, int j)
            {
                if (i == j)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (i > j)
                {
                    var temp = i;
                    i = j;
                    j = temp;
                }
                return n * (n - 1) / 2 - (n - i) * (n - i - 1) / 2 + j - i - 1;
            }
        }

        private class MatrixCalculator
        {
            private int?[] lemmasCountCache;
            private int?[] niCache;

            public Matrix Matrix(Text[] corpus, string[] allLemmas, string[] uniqueLemmas)
            {
                lemmasCountCache = new int?[uniqueLemmas.Length];
                niCache = new int?[uniqueLemmas.Length];

                var matrix = new Matrix(uniqueLemmas.Length);

                Parallel.For(0, uniqueLemmas.Length, (int i) =>
                {
                    Parallel.For(i + 1, uniqueLemmas.Length, (int j) =>
                    {
                        var cij = GetCorrelation(corpus, allLemmas, uniqueLemmas, i, j);
                        var cji = GetCorrelation(corpus, allLemmas, uniqueLemmas, j, i);
                        var max = Math.Max(cij, cji);
                        var min = Math.Min(cij, cji);

                        matrix[i, j] = max == 1 ? min : max;
                    });
                });

                return matrix;
            }

            private double GetCorrelation(Text[] corpus, string[] allLemmas, string[] uniqueLemmas, int i, int j)
            {
                if (niCache[i] == null)
                {
                    niCache[i] = corpus
                        .Where(x => x.Terms.Contains(uniqueLemmas[i]))
                        .SelectMany(x => x.Terms)
                        .Count();
                }
                if (lemmasCountCache[j] == null)
                {
                    lemmasCountCache[j] = allLemmas
                        .Count(x => x == uniqueLemmas[j]);
                }

                int ni = niCache[i].Value;
                int nj = lemmasCountCache[j].Value;
                int nij = corpus
                    .Where(x => x.Terms.Contains(uniqueLemmas[i]))
                    .SelectMany(x => x.Terms)
                    .Count(x => x == uniqueLemmas[j]);

                if (nij <= ni * nj / allLemmas.Length)
                    return 1;
                else
                    return Math.Pow((double)ni /
                        allLemmas.Length, nij) * Math.Pow(1 - (double)ni / allLemmas.Length, nj - nij) * BinomCoefficient(nj, nij);
            }

            private double BinomCoefficient(double n, double k)
            {
                if (k > n) { return 0; }
                if (n == k) { return 1; } // only one way to chose when n == k
                if (k > n - k) { k = n - k; } // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
                double c = 1;
                for (double i = 1; i <= k; i++)
                {
                    c *= n--;
                    c /= i;
                }
                return c;
            }
        }

        private class Cluster<T> : IEnumerable<T>
        {
            private List<T> items = new List<T>();

            public void Add(T item)
            {
                items.Add(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        private class Medoid
        {
            public int Center { get; set; }
            public List<int> Points { get; set; } = new List<int>();
        }

    }
}
