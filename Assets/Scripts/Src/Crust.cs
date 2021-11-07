using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;
using static RNGalaxy.Plate;

namespace RNGalaxy
{
    public class Crust
    {
        public int numberOfPlates;
        public float radius;
        public int randomSeed;

        public List<Plate> plates;

        public Crust(int numberOfPlates, float radius, Mesh sampleMesh, int randomSeed = 42)
        {
            this.numberOfPlates = numberOfPlates;
            this.radius = radius;
            this.randomSeed = randomSeed;
            plates = GeneratePlates();
            SampleMesh(sampleMesh);
        }

        private HashSet<VoronoiCell3> GenerateVoronoiCells(HashSet<Vector3> points)
        {
            HashSet<MyVector3> convertedPoints = points.ToMyVector3();
            List<MyVector3> unitBoundingBox = new List<MyVector3>(convertedPoints);
            Normalizer3 normalizer = new Normalizer3(unitBoundingBox);

            // Normalize all points.
            HashSet<MyVector3> normalizedPoints = normalizer.Normalize(convertedPoints);

            // Generate a convex hull using the normalized points
            HalfEdgeData3 convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(normalizedPoints, false);

            HashSet<VoronoiCell3> voronoiCells = Delaunay3DToVoronoiAlgorithm.GenerateVoronoiDiagram(convexHull);

            // De-normalize the voronoi diagram.
            voronoiCells = normalizer.UnNormalize(voronoiCells);

            return voronoiCells;
        }

        private List<Plate> GeneratePlates()
        {
            Random.InitState(randomSeed);
            HashSet<Vector3> points = HelperMethods.GenerateMixedPointClustersOnSphere(numberOfPlates, radius);
            
            HashSet<VoronoiCell3> voronoiCells = GenerateVoronoiCells(points);

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

            return plates;
        }

        private Plate FindOtherEdgeParent(PlateEdge childEdge, Plate parentPlate, List<Plate> plates)
        {

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

        private void SampleMesh(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;

            // Loop over all vertices.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];


                // Find the nearest plate to this vertex.
                // 
                // Get an initial nearest plate, and its distance from the vertex.
                Plate nearestPlate = plates[0];
                float smallestDistance = Vector3.Distance(vertex, nearestPlate.center);

                // Search through all plates.
                foreach (Plate plate in plates)
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
    }
}