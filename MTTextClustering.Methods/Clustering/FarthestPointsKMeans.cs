using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods.Clustering
{
    // https://github.com/ArturVasyliev/Clustering/blob/master/Clustering/Clustering.Kmeans.cs
    public class FarthestPointsKMeans : IClusteringMethod, IKClusteringMethod
    {
        public List<List<Guid>> Clusterize(Text[] texts, int k, Dictionary<string, object> @params)
        {
            if (k < 1)
            {
                throw new Exception("K must be greater than 0");
            }

            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();
            var points = tfIdf.Select(x => new DataPoint(x)).ToArray();

            var centroids = GetFarthestPoint(points, k);

            var centroidsChanged = true;
            var counter = 0;

            while (centroidsChanged)
            {
                for (int i = 0; i < points.Count(); i++)
                {
                    double minDist = int.MaxValue;
                    double dist = 0;
                    for (int c = 0; c < k; c++)
                    {
                        dist = points[i].GetDistance(centroids[c]);
                        if (dist < minDist)
                        {
                            points[i].Cluster = c;
                            minDist = dist;
                        }
                    }
                }

                int[] clusterSize = new int[k];
                double[][] sum = new double[k][];

                for (int i = 0; i < k; i++)
                {
                    sum[i] = new double[points[0].Vector.Length];
                }

                //calculating mean point for each cluster
                for (int i = 0; i < points.Count(); i++)
                {
                    int currCluster = points[i].Cluster;
                    clusterSize[currCluster]++;

                    for (int j = 0; j < points[currCluster].Vector.Length; j++)
                    {
                        sum[currCluster][j] += points[i].Vector[j];
                    }
                }

                int centroidsChangedCounter = 0;

                //setting new mean points as centroids
                for (int i = 0; i < k; i++)
                {
                    DataPoint dp = new DataPoint(sum[i].Select(x => x / clusterSize[i]).ToArray());

                    if (dp.Vector.Any(x => double.IsNaN(x)))
                    {
                        throw new Exception("K value is too large for this set");
                    }
                    else if (dp != centroids[i])
                    {
                        centroids[i] = dp;
                        centroidsChangedCounter++;
                    }
                }

                //if new centroids are the same as old - stop algorithm
                if (centroidsChangedCounter == 0)
                {
                    centroidsChanged = false;
                }

                counter++;
            }

            var result = Enumerable.Range(0, k).Select(x => new List<Guid>()).ToList();

            for (int i = 0; i < points.Length; i++)
            {
                result[points[i].Cluster].Add(texts[i].Id);
            }

            return result;
        }

        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            return Clusterize(texts, (int)@params["k"], @params);
        }

        private DataPoint[] GetFarthestPoint(DataPoint[] points, int k)
        {
            var pointsAndDistances = new List<(double sumDistance, DataPoint[] points)>();

            foreach (var point in points)
            {
                var farthestPoints = points
                    .Select(x => new { Point = x, Distance = point.GetDistance(x) })
                    .OrderByDescending(x => x.Distance)
                    .Take(k - 1)
                    .ToList();

                farthestPoints.Insert(0, new { Point = point, Distance = 0.0 });

                pointsAndDistances.Add(new(
                    farthestPoints.Sum(x => x.Distance),
                    farthestPoints.Select(x => x.Point).ToArray()));
            }

            return pointsAndDistances
                .OrderByDescending(x => x.sumDistance)
                .First()
                .points;
        }

        private class DataPoint
        {
            public double[] Vector { get; set; }

            public int Cluster { get; set; }

            public DataPoint(double[] vector)
            {
                Vector = vector;
            }

            public double GetDistance(DataPoint other)
            {
                var sum = 0.0;

                for (int i = 0; i < Vector.Length; i++)
                {
                    sum += Math.Pow(Vector[i] - other.Vector[i], 2);
                }

                return Math.Sqrt(sum);
            }

            public override bool Equals(object obj)
            {
                var point = (DataPoint)obj;
                var res = true;

                for (int i = 0; i < Vector.Length; i++)
                {
                    res &= Math.Abs(Vector[i] - point.Vector[i]) < 0.00001;
                }

                return res;
            }

            public static bool operator ==(DataPoint p1, DataPoint p2)
            {
                return p1.Equals(p2);
            }

            public static bool operator !=(DataPoint p1, DataPoint p2)
            {
                return !p1.Equals(p2);
            }
        }
    }
}
