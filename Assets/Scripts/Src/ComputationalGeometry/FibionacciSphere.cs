using Habrador_Computational_Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public class FibionacciSphere
    {
        public static Vector3[] GeneratePoints(int n, float radius, float jitter = 0)
        {
            Vector3[] points = new Vector3[n];

            // Generate n fibionacci sphere points.
            for (int i = 0; i < n; i++)
            {
                Vector3 point = FibSpherePoint(i, n, radius, jitter);
                points[i] = point;
            }

            return points;
        }

        public static HashSet<MyVector3> GeneratePointSet(int n, float radius, float jitter = 0)
        {
            HashSet<MyVector3> points = new HashSet<MyVector3>();

            // Generate n fibionacci sphere points.
            for (int i = 0; i < n; i++)
            {
                Vector3 p = FibSpherePoint(i, n, radius, jitter);
                MyVector3 point = p.ToMyVector3();
                points.Add(point);
            }

            return points;
        }

        private static Vector3 FibSpherePoint(int i, int n, float radius, float jitter)
        {
            var k = i + .5f;

            var phi = Mathf.Acos(1f - 2f * k / n);
            var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

            var x = Mathf.Cos(theta) * Mathf.Sin(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(phi);
            Vector3 fibSpherePoint = new Vector3(x, y, z);

            float randX = Random.Range(-1, 1);
            float randY = Random.Range(-1, 1);
            float randZ = Random.Range(-1, 1);
            Vector3 pointJitter = new Vector3(randX, randY, randZ).normalized * jitter / (n/100);
            return (fibSpherePoint + pointJitter).normalized * radius;
        }
    }
}