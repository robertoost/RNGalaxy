using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public static class HelperMethods
    {
        // https://answers.unity.com/questions/58692/randomonunitsphere-but-within-a-defined-range.html
        public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
        {
            float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
            Vector2 PointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
            Vector3 V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));

            return targetDirection * V;
        }
        public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
        {
            return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
        }

        public static HashSet<Vector3> GenerateRandomPointsOnSphere(int n, float radius)
        {
            HashSet<Vector3> points = new HashSet<Vector3>();

            for (int i = 0; i < n; i++)
            {
                Vector3 point = Random.onUnitSphere * radius;
                points.Add(point);
            }

            return points;
        }

        public static HashSet<Vector3> GeneratePointClustersOnSphere(int numPoints, float radius)
        {
            float clusterRadius = 20f;
            int maxClusterSize = numPoints / 2;
            int numPointsLeft = numPoints;
            HashSet<Vector3> points = new HashSet<Vector3>();

            while (numPointsLeft > 0)
            {
                Vector3 point = Random.onUnitSphere * radius;

                // Get the number of points we want to use in the cluster.
                int clusterSize = Mathf.Min(maxClusterSize, numPointsLeft) + 1;
                int numClusterPoints = Random.Range(1, clusterSize);
                
                for(int i=0; i < numClusterPoints; i++)
                {
                    Vector3 clusterPoint = GetPointOnUnitSphereCap(point, clusterRadius) * radius;
                    points.Add(clusterPoint);
                    numPointsLeft--;
                }
            }

            return points;
        }


        // Extension methods
        public static HashSet<MyVector3> ToMyVector3(this HashSet<Vector3> vectors)
        {
            HashSet<MyVector3> myVectors = new HashSet<MyVector3>();
            foreach (Vector3 vector in vectors)
            {
                myVectors.Add(vector.ToMyVector3());
            }
            return myVectors;
        }
    }
}
