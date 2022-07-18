using MTTextClustering.Methods.Contracts;
using MTTextClustering.Methods.Helpers;
using MTTextClustering.Models;

namespace MTTextClustering.Methods
{
    // https://github.com/yusufuzun/dbscan/blob/master/DbscanImplementation/DbscanAlgorithm.cs
    public class Dbscan : IClusteringMethod
    {
        public List<List<Guid>> Clusterize(Text[] texts, Dictionary<string, object> @params)
        {
            var epsilon = (double)@params["epsilon"];
            var minimumPoints = (int)@params["minimumPoints"];
            var tfIdf = new TfIdfCalculator(texts).GetTfIdfMatrix();

            var allPointsDbscan = texts.Select((x, i) => new DbscanPoint(tfIdf[i], x.Id)).ToArray();

            int clusterId = 0;

            for (int i = 0; i < allPointsDbscan.Length; i++)
            {
                var currentPoint = allPointsDbscan[i];

                if (currentPoint.PointType.HasValue)
                {
                    continue;
                }

                var neighborPoints = RegionQuery(allPointsDbscan, currentPoint, epsilon);

                if (neighborPoints.Length < minimumPoints)
                {
                    currentPoint.PointType = PointType.Noise;

                    continue;
                }

                clusterId++;

                currentPoint.ClusterId = clusterId;

                currentPoint.PointType = PointType.Core;

                ExpandCluster(allPointsDbscan, neighborPoints, clusterId, epsilon, minimumPoints);
            }

            return allPointsDbscan
                .Where(x => x.PointType != PointType.Noise)
                .GroupBy(x => x.ClusterId)
                .Select(x => x.Select(y => y.Id).ToList())
                .ToList();
        }

        private DbscanPoint[] RegionQuery(DbscanPoint[] allPoints, DbscanPoint mainFeature, double epsilon)
        {
            return allPoints.Where(x => x.GetDistance(mainFeature) <= epsilon).ToArray();
        }

        private void ExpandCluster(DbscanPoint[] allPoints, DbscanPoint[] neighborPoints,
            int clusterId, double epsilon, int minimumPoints)
        {
            for (int i = 0; i < neighborPoints.Length; i++)
            {
                var currentPoint = neighborPoints[i];

                if (currentPoint.PointType == PointType.Noise)
                {
                    currentPoint.ClusterId = clusterId;

                    currentPoint.PointType = PointType.Border;

                    continue;
                }

                if (currentPoint.PointType.HasValue)
                {
                    continue;
                }

                currentPoint.ClusterId = clusterId;

                var otherNeighborPoints = RegionQuery(allPoints, currentPoint, epsilon);

                if (otherNeighborPoints.Length < minimumPoints)
                {
                    currentPoint.PointType = PointType.Border;

                    continue;
                }

                currentPoint.PointType = PointType.Core;

                neighborPoints = neighborPoints.Union(otherNeighborPoints).ToArray();
            }
        }

        private class DbscanPoint
        {
            public double[] Vector { get; set; }

            public Guid Id { get; set; }

            public int? ClusterId { get; set; }

            public PointType? PointType { get; set; }

            public DbscanPoint(double[] vector, Guid id)
            {
                Vector = vector;
                Id = id;
                ClusterId = null;
                PointType = null;
            }

            public double GetDistance(DbscanPoint other)
            {
                var distance = 0.0;

                for (int i = 0; i < Vector.Length; i++)
                {
                    distance += Math.Pow(Vector[i] - other.Vector[i], 2);
                }

                distance = Math.Sqrt(distance);

                return distance;
            }
        }

        private enum PointType
        {
            Noise = 0,
            Core = 1,
            Border = 2
        }
    }
}
