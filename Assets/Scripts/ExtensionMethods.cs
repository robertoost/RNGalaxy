using Habrador_Computational_Geometry;
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
    }
}