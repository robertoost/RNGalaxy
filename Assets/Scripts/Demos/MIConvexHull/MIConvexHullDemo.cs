using System.Collections.Generic;
using UnityEngine;
using MIConvexHull;

namespace RNGalaxy
{
    public class MIConvexHullDemo : MonoBehaviour
    {
        public int randomSeed = 42;

        [Range(100, 10000)]
        public int numPoints = 100;

        [Range(4f, 16f)]
        public float radius = 8f;

        [Range(0, 1)]
        public float jitter = 0f;

        private ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull;
        // Start is called before the first frame update
        void OnValidate()
        {
            Random.InitState(randomSeed);
            //Stopwatch stopwatch = Stopwatch.StartNew();
            Vector3[] points = FibionacciSphere.GeneratePoints(numPoints, 8, jitter);
            List<double[]> convertedPoints = ConvertPoints(points);

            convexHull = GenerateConvexHull(convertedPoints);
        }

        private ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> GenerateConvexHull(List<double[]> points)
        {
            var convexHull = ConvexHull.Create(points).Result;
            return convexHull;
        }

        private List<double[]> ConvertPoints(Vector3[] points)
        {
            List<double[]> convertedPoints = new List<double[]>();
            foreach (Vector3 point in points)
            {
                double[] pointData = Vector3ToDefaultVertex(point).Position;
                convertedPoints.Add(pointData);
            }

            return convertedPoints;
        }

        private Vector3 DefaultVertexToVector3(DefaultVertex point)
        {
            double[] pos = point.Position;
            Vector3 convertedPoint = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
            return convertedPoint;
        }

        private DefaultVertex Vector3ToDefaultVertex(Vector3 point)
        {
            DefaultVertex convertedPoint = new DefaultVertex();
            convertedPoint.Position = new double[] { point.x, point.y, point.z };
            return convertedPoint;
        }

        // Update is called once per frame
        void OnDrawGizmos()
        {
            DrawConvexHull();
        }

        void DrawConvexHull()
        {
            // Draw all points.

            //Gizmos.color = Color.blue;
            //foreach (DefaultVertex point in convexHull.Points)
            //{
            //    Vector3 convertedPoint = DefaultVertexToVector3(point);
            //    Gizmos.DrawSphere(convertedPoint.normalized * radius, 0.1f);
            //}

            Gizmos.color = Color.white;

            int[][] vertexCombinations = new int[][] { new int[] { 0, 1 }, new int[] { 1, 2 }, new int[] { 2, 0 } };

            foreach (DefaultConvexFace<DefaultVertex> face in convexHull.Faces)
            {
                foreach (int[] pair in vertexCombinations)
                {
                    Vector3 p1 = DefaultVertexToVector3(face.Vertices[pair[0]]);
                    Vector3 p2 = DefaultVertexToVector3(face.Vertices[pair[1]]);
                    p1 = p1.normalized * radius;
                    p2 = p2.normalized * radius;

                    Gizmos.DrawLine(p1, p2);
                }
            }

        }
    }
}
