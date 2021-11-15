using Habrador_Computational_Geometry;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public class SphereMeshGenerator : MonoBehaviour
    {
        public int vertices = 10000;
        public float radius = 8f;
        
        // Start is called before the first frame update
        void Start()
        {
            // Convert points to a list of arrays containing doubles.
            Vector3[] points = FibonacciSphere.GeneratePoints(vertices, radius, 0);
            List<double[]> convertedPoints = points.ToDoubleArrayList();

            // Generate a quick convex hull.
            var convexHull = ConvexHull.Create(convertedPoints).Result;
            HalfEdgeData3 halfEdgeData = HalfEdgeMapper.MapHalfEdgeData(convexHull);

            string meshName = $"Planet{vertices}";
            Mesh mesh = halfEdgeData.ConvertToMyMesh(meshName, MyMesh.MeshStyle.SoftEdges).ConvertToUnityMesh(true, meshName);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }
    }
}
