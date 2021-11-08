using System.Collections.Generic;
using UnityEngine;
using MIConvexHull;
using Habrador_Computational_Geometry;
using System;
using System.Diagnostics;
using static Habrador_Computational_Geometry.Delaunay3DToVoronoiAlgorithm;

namespace RNGalaxy
{
    public class MIVoronoiSphereDemo : MonoBehaviour
    {
        public int randomSeed = 42;

        [Range(100, 10000)]
        public int numPoints = 100;

        [Range(4f, 16f)]
        public float radius = 8f;

        [Range(0, 1)]
        public float jitter = 0f;

        // Of course we want this.
        public bool UseMIConvexHull = true;

        // Is probably a bit slower, but is necessary for neighbor-checking.
        public bool StoreVoronoiNeighborData = true;

        

        private ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull;
        private HashSet<VoronoiCell3> voronoiCells;
        private HashSet<VoronoiTile> voronoiTiles;
        private List<HalfEdgeFace3> halfEdgeFaceList;


        // Start is called before the first frame update
        void OnValidate()
        {
            UnityEngine.Random.InitState(randomSeed);
            
            // Convert points to a list of arrays containing doubles.
            Vector3[] points = FibionacciSphere.GeneratePoints(numPoints, 8, jitter);
            List<double[]> convertedPoints = points.ToDoubleArrayList();

            Stopwatch stopwatch = Stopwatch.StartNew();

            HalfEdgeData3 halfEdgeData;

            if (UseMIConvexHull)
            {
                // Generate a quick convex hull.
                convexHull = ConvexHull.Create(convertedPoints).Result;

                // Generate half edge data from the convex hull for use in the voronoi diagram.
                halfEdgeData = HalfEdgeMapper.MapHalfEdgeData(convexHull);

            } else
            {
                halfEdgeData = IterativeHullAlgorithm3D.GenerateConvexHull(new HashSet<Vector3>(points).ToMyVector3(), false);
            }
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Generating convex hull for {stopwatch.ElapsedMilliseconds / 1000f} seconds.");

            stopwatch.Restart();
            // Generate voronoi cells
            if (StoreVoronoiNeighborData)
            {
                voronoiTiles = Delaunay3DToVoronoiTile.GenerateVoronoiDiagram(halfEdgeData, center: Delaunay3DToVoronoiTile.VoronoiCenter.Centroid);
            } else
            {
                voronoiCells = GenerateVoronoiDiagram(halfEdgeData, center:VoronoiCenter.Centroid);
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log($"Generating voronoi diagram for {stopwatch.ElapsedMilliseconds / 1000f} seconds.");
        }

        private void OnDrawGizmos()
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            if (StoreVoronoiNeighborData)
            {
                DrawVoronoiTiles(pos, rot);
                HighlightTileNeighbors(pos, rot);
            }
            else
            {
                DrawVoronoiCells(pos, rot);

            }
        }

        private void DrawVoronoiTiles(Vector3 pos, Quaternion rot)
        {
            foreach (VoronoiTile tile in voronoiTiles)
            {
                foreach (VoronoiTileEdge edge in tile.edges)
                {

                    Vector3 p1 = edge.p1.ToVector3();
                    Vector3 p2 = edge.p2.ToVector3();
                    p1 = pos + rot * (p1.normalized * radius);
                    p2 = pos + rot * (p2.normalized * radius);

                    Gizmos.DrawLine(p1, p2);
                }
            }
        }

        private void HighlightTileNeighbors(Vector3 pos, Quaternion rot)
        {
            foreach (VoronoiTile tile in voronoiTiles)
            {
                Gizmos.color = Color.red;
                foreach (VoronoiTileEdge edge in tile.edges)
                {

                    Vector3 p1 = edge.p1.ToVector3();
                    Vector3 p2 = edge.p2.ToVector3();
                    p1 = pos + rot * (p1.normalized * radius);
                    p2 = pos + rot * (p2.normalized * radius);

                    Gizmos.DrawLine(p1, p2);

                    Vector3 neighborPos = edge.oppositeTile.sitePos.ToVector3();
                    neighborPos = pos + rot * (neighborPos.normalized * radius);

                    Gizmos.DrawSphere(neighborPos, 0.05f);
                }

                break;
            }
        }

        private void DrawVoronoiCells(Vector3 pos, Quaternion rot)
        {
            foreach (VoronoiCell3 cell in voronoiCells)
            {
                foreach (VoronoiEdge3 edge in cell.edges)
                {

                    Vector3 p1 = edge.p1.ToVector3();
                    Vector3 p2 = edge.p2.ToVector3();
                    p1 = pos + rot * (p1.normalized * radius);
                    p2 = pos + rot * (p2.normalized * radius);

                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
    }
}
