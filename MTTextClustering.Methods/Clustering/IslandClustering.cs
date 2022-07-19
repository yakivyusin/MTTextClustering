using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;
using System.Collections;

namespace MTTextClustering.Methods.Clustering
{
    public class IslandClustering : IClusteringMethod
    {
        private List<TermsIsland> growingIslands = new List<TermsIsland>();
        private List<TermsIsland> fixedIslands = new List<TermsIsland>();
        private List<int> nonboundingTerms = new List<int>();
        private string[] allLemmas;
        private string[] uniqueLemmas;

        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            growingIslands.Clear();
            fixedIslands.Clear();
            nonboundingTerms.Clear();
            allLemmas = texts.SelectMany(x => x.Terms).ToArray();
            uniqueLemmas = allLemmas.Distinct().ToArray();

            var ts = (int)@params["ts"];
            var matrix = new MatrixCalculator().Matrix(texts, allLemmas, uniqueLemmas);
            var matrixThreshold = 100.0 / uniqueLemmas.Length;

            var filteredMatrix = matrix.Where(x => x.Correlation <= matrixThreshold).ToList();
            var termClusters = MatrixClustering(filteredMatrix, texts, ts);
            var textClusters = MapClusters(termClusters, texts);

            return textClusters
                .Select(x => x.Select(y => y.Id).ToList())
                .ToList();
        }

        private List<Cluster<string>> MatrixClustering(List<MatrixLink> filteredMatrix, Text[] texts, int ts)
        {
            var links = filteredMatrix
                .OrderBy(x => x.Correlation)
                .ToList();

            BuildIslands(links, texts, ts);

            for (int i = growingIslands.Count - 1; i >= 0; i--)
            {
                FixIsland(growingIslands[i]);
            }

            return fixedIslands.Select(island =>
            {
                var cluster = new Cluster<string>();
                island.Terms.ForEach(t => cluster.Add(uniqueLemmas[t]));
                return cluster;
            }).ToList();
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

            var clustersToRemove = new List<Cluster<Text>>();
            foreach (var cluster in clusters)
            {
                foreach (var cluster2 in clusters.Where(x => x != cluster))
                {
                    if (cluster.All(t1 => cluster2.Contains(t1)))
                    {
                        clustersToRemove.Add(cluster);
                        break;
                    }
                }
            }

            clusters.RemoveAll(x => clustersToRemove.Contains(x));

            return clusters;
        }

        private void BuildIslands(List<MatrixLink> links, Text[] texts, int ts)
        {
            foreach (var link in links)
            {
                if (nonboundingTerms.Contains(link.Row) && nonboundingTerms.Contains(link.Column))
                {
                }
                else if (nonboundingTerms.Contains(link.Row))
                {
                    var island = growingIslands.FirstOrDefault(x => x.Terms.Contains(link.Column));
                    if (island != null)
                    {
                        FixIsland(island);
                    }
                    else
                    {
                        nonboundingTerms.Add(link.Column);
                    }
                }
                else if (nonboundingTerms.Contains(link.Column))
                {
                    var island = growingIslands.FirstOrDefault(x => x.Terms.Contains(link.Row));
                    if (island != null)
                    {
                        FixIsland(island);
                    }
                    else
                    {
                        nonboundingTerms.Add(link.Row);
                    }
                    continue;
                }
                else if (!growingIslands.Any(x => x.Terms.Contains(link.Row)) &&
                         !growingIslands.Any(x => x.Terms.Contains(link.Column)))
                {
                    var newIsland = new TermsIsland();
                    newIsland.Terms.Add(link.Row);
                    newIsland.Terms.Add(link.Column);
                    newIsland.Links.Add(link);
                    growingIslands.Add(newIsland);
                }
                else if (growingIslands.Any(x => x.Terms.Contains(link.Row)) &&
                         !growingIslands.Any(x => x.Terms.Contains(link.Column)))
                {
                    var island = growingIslands.First(x => x.Terms.Contains(link.Row));
                    island.Links.Add(link);
                    island.Terms.Add(link.Column);
                }
                else if (!growingIslands.Any(x => x.Terms.Contains(link.Row)) &&
                         growingIslands.Any(x => x.Terms.Contains(link.Column)))
                {
                    var island = growingIslands.First(x => x.Terms.Contains(link.Column));
                    island.Links.Add(link);
                    island.Terms.Add(link.Row);
                }
                else if (growingIslands.First(x => x.Terms.Contains(link.Row)) ==
                         growingIslands.First(x => x.Terms.Contains(link.Column)))
                {
                    growingIslands.First(x => x.Terms.Contains(link.Row)).Links.Add(link);
                }
                else if (growingIslands.First(x => x.Terms.Contains(link.Row)) !=
                         growingIslands.First(x => x.Terms.Contains(link.Column)))
                {
                    var rowIsland = growingIslands.First(x => x.Terms.Contains(link.Row));
                    var columnIsland = growingIslands.First(x => x.Terms.Contains(link.Column));
                    if (PopIsland(rowIsland, texts) > ts || PopIsland(columnIsland, texts) > ts)
                    {
                        FixIsland(rowIsland);
                        FixIsland(columnIsland);
                    }
                    else
                    {
                        growingIslands.Remove(rowIsland);
                        growingIslands.Remove(columnIsland);
                        var union = new TermsIsland
                        {
                            Terms = rowIsland.Terms.Union(columnIsland.Terms).ToList(),
                            Links = rowIsland.Links.Union(columnIsland.Links).ToList()
                        };
                        union.Links.Add(link);
                        growingIslands.Add(union);
                    }
                }
            }
        }

        private void FixIsland(TermsIsland island)
        {
            growingIslands.Remove(island);
            fixedIslands.Add(island);
            nonboundingTerms.AddRange(island.Terms);
        }

        private int PopIsland(TermsIsland island, Text[] texts)
        {
            int sum = 0;
            foreach (var text in texts)
            {
                if (island.Links.Any(x => text.Terms.Contains(uniqueLemmas[x.Row])
                && text.Terms.Contains(uniqueLemmas[x.Column])))
                    sum++;
            }
            return sum;
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

                Parallel.For(0, uniqueLemmas.Length, (i) =>
                {
                    Parallel.For(i + 1, uniqueLemmas.Length, (j) =>
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
    }
}
