using Habrador_Computational_Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    [RequireComponent(typeof(MeshFilter))]
    public class ConvexHullDemo : MonoBehaviour
    {
        public int n = 200;
        public float radius = 8;
        private HashSet<MyVector3> points;
        private HalfEdgeData3 convexHull;

        private void Start()
        {
            points = FibonacciSphere.GeneratePointSet(n, radius);
            convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(points, true);

            Mesh mesh = convexHull.ConvertToMyMesh("Planet", MyMesh.MeshStyle.HardAndSoftEdges).ConvertToUnityMesh(true, "Planet");
            
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
    }

}
