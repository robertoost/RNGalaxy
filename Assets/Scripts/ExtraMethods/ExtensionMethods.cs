using Habrador_Computational_Geometry;
using MIConvexHull;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public static class ExtensionMethods
    {

        public static HashSet<MyVector3> ToMyVector3(this HashSet<Vector3> vectors)
        {
            HashSet<MyVector3> myVectors = new HashSet<MyVector3>();
            foreach (Vector3 vector in vectors)
            {
                myVectors.Add(vector.ToMyVector3());
            }
            return myVectors;
        }

        public static MyVector3 ToMyVector3(this DefaultVertex point)
        {
            double[] position = point.Position;
            return position.ToMyVector3();
        }

        public static MyVector3 ToMyVector3(this double[] position)
        {
            return new MyVector3((float)position[0], (float)position[1], (float)position[2]);
        }

        public static DefaultVertex ToDefaultVertex(this Vector3 point)
        {
            DefaultVertex convertedPoint = new DefaultVertex();
            convertedPoint.Position = new double[] { point.x, point.y, point.z };
            return convertedPoint;
        }

        public static List<double[]> ToDoubleArrayList(this Vector3[] points)
        {
            List<double[]> convertedPoints = new List<double[]>();
            foreach (Vector3 point in points)
            {
                double[] pointData = point.ToDefaultVertex().Position;
                convertedPoints.Add(pointData);
            }

            return convertedPoints;
        }



    }
}