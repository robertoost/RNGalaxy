using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;
using static RNGalaxy.Plate;

namespace RNGalaxy
{
    public class CrustGenerator : MonoBehaviour
    {
        public int seed = 20;
        public int n = 30;
        public float radius = 8;

        public HashSet<Vector3> points;
        public List<Plate> plates;

        private MeshFilter meshFilter;
        private Mesh originalMesh;

        // Update is called once per frame
        void OnValidate()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (!originalMesh)
            {
                originalMesh = Instantiate<Mesh>(meshFilter.sharedMesh);
            }
            Random.InitState(seed);
            HashSet<Vector3> points = GeneratePoints();
            HashSet<VoronoiCell3> cells = GenerateVoronoiCells(points);
            plates = GeneratePlates(cells);
            if (Application.isPlaying)
            {
                RaiseVertices(plates);
            }
            //{
            //    //meshFilter = GetComponent<MeshFilter>();
            //    //originalMesh = Instantiate<Mesh>(meshFilter.sharedMesh);
            //}
        }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            originalMesh = Instantiate<Mesh>(meshFilter.sharedMesh);
            RaiseVertices(plates);
        }

        private void RaiseVertices(List<Plate> plates)
        {
            Vector3[] vertices = originalMesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            foreach(Plate plate in plates)
            {
                foreach(int vertexID in plate.vertices)
                {

                    Vector3 vertex = vertices[vertexID];
                    newVertices[vertexID] = plate.plateType == PlateType.oceanic ? vertex - vertex.normalized * 0.03f : vertex;
                }
            }
            meshFilter.mesh.vertices = newVertices;
            meshFilter.mesh.RecalculateNormals();
        }

        private void SaveVertices(List<Plate> plates)
        {
            Vector3[] vertices = originalMesh.vertices;

            // Loop over all vertices.
            for(int i=0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];


                // Find the nearest plate to this vertex.
                // 
                // Get an initial nearest plate, and its distance from the vertex.
                Plate nearestPlate = plates[0];
                float smallestDistance = Vector3.Distance(vertex, nearestPlate.center);

                // Search through all plates.
                foreach(Plate plate in plates)
                {
                    // Compare distances. If this distance is larger than the recorded smallest distance, move on to the next plate.
                    float currentDistance = Vector3.Distance(vertex, plate.center);
                    if (currentDistance > smallestDistance)
                        continue;

                    // This plate is the nearest plate.
                    nearestPlate = plate;
                    smallestDistance = currentDistance;
                }

                nearestPlate.vertices.Add(i);
            }
        }

        /// <summary>
        /// Generates a number n of tectonic plates, with 2/3rds being small clustered plates, and 1/3rd being big plates.
        /// </summary>
        /// <returns>A hashset of Vector3 points.</returns>
        private HashSet<Vector3> GeneratePoints()
        {
            int quotient = n / 3;
            int remainder = n % 3;

            int numBigPlates = quotient;
            int numSmallPlates = 2 * quotient + remainder;

            HashSet<Vector3> randomPoints = HelperMethods.GenerateRandomPointsOnSphere(numBigPlates, radius);
            HashSet<Vector3> randomClusters = HelperMethods.GeneratePointClustersOnSphere(numSmallPlates, radius);
            HashSet<Vector3> points = new HashSet<Vector3>(randomPoints);
            points.UnionWith(randomClusters);

            return points;
        }

        private HashSet<VoronoiCell3> GenerateVoronoiCells(HashSet<Vector3> points)
        {
            HalfEdgeData3 convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(points.ToMyVector3(), true);
            HashSet<VoronoiCell3> voronoiCells = Delaunay3DToVoronoiAlgorithm.GenerateVoronoiDiagram(convexHull);
            return voronoiCells;
        }

        private List<Plate> GeneratePlates(HashSet<VoronoiCell3> voronoiCells)
        {

            List<Plate> plates = new List<Plate>();

            foreach (VoronoiCell3 cell in voronoiCells)
            {
                Plate plate = new Plate(cell.sitePos.ToVector3(), (PlateType)Random.Range(0, 2), cell.edges);
                plates.Add(plate);
            }

            foreach (Plate plate in plates)
            {
                foreach (PlateEdge plateEdge in plate.edges)
                {
                    // If this plate already has all its parents, move on.
                    if (plateEdge.parents[1] != null)
                        continue;

                    Plate plateNeighbor = FindOtherEdgeParent(plateEdge, plate, plates);
                    plateEdge.parents[1] = plateNeighbor;
                }
            }

            SaveVertices(plates);

            return plates;
        }

        private Plate FindOtherEdgeParent(PlateEdge childEdge, Plate parentPlate, List<Plate> plates) {

            foreach (Plate plate in plates)
            {
                if (plate == parentPlate)
                    continue;

                Plate neighborPlate = null;

                foreach (PlateEdge plateEdge in plate.edges)
                {
                    // Compare both ways.
                    bool forwardSameEdge = childEdge.start == plateEdge.start && childEdge.end == plateEdge.end;
                    bool backwardSameEdge = childEdge.start == plateEdge.end && childEdge.end == plateEdge.start;

                    // Move on if there is no match
                    if (!forwardSameEdge && !backwardSameEdge)
                        continue;

                    neighborPlate = plate;
                }

                // If this plate is my neighbor, return it.
                if (neighborPlate != null)
                    return neighborPlate;
            }

            // Code should not reach this point.
            throw new System.Exception("No neighbor found!");
        }


        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.red;
            //foreach(Vector3 point in randomPoints)
            //{
            //    Vector3 drawPoint = transform.rotation * point + transform.position;
            //    Gizmos.DrawSphere(drawPoint, 0.2f);
            //}

            //Gizmos.color = Color.red;

            //MeshFilter meshFilter = GetComponent<MeshFilter>();
            //Vector3[] vertices = meshFilter.sharedMesh.vertices;
            //Plate plate = plates[0];

            //foreach (int vertexID in plate.vertices)
            //{
            //    Vector3 vertex = vertices[vertexID];
            //    //Vector3 drawPoint = transform.rotation * vertex + transform.position;
            //    Vector3 drawPoint = transform.rotation * (radius * vertex.normalized) + transform.position;
            //    Gizmos.DrawSphere(drawPoint, 0.05f);
            //}
        }

    }
}