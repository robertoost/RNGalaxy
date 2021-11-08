using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Habrador_Computational_Geometry;
using System.Diagnostics;
using static UnityEngine.Debug;
using static Habrador_Computational_Geometry.Delaunay3DToVoronoiAlgorithm;

namespace RNGalaxy
{
    public class FibionacciVoronoiSphere : MonoBehaviour
    {
        public int randomSeed = 42;
        [Range(100, 10000)]
        public int numPoints = 100;
        public float radius = 8f;
        [Range(0, 1)]
        public float jitter = 0.5f;

        public VoronoiCenter centerType;
        public bool removeInnerPoints = true;

        private Vector3[] points;
        private HashSet<VoronoiCell3> voronoiCells;
        private HalfEdgeData3 convexHull;
        

        // Start is called before the first frame update
        void OnValidate()
        {
            // Only validate input during play mode.
            if (!Application.isPlaying)
                return;

            Random.InitState(randomSeed);

            GenerateSphere();


            //transform.localScale = Vector3.one * radius * 2;
        }

        // Update is called once per frame
        private void GenerateSphere()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Convert the points to MyVector3 and then normalize every point.
            points = FibionacciSphere.GeneratePoints(numPoints, radius, jitter: jitter);

            HashSet<MyVector3> pointsSet = new HashSet<Vector3>(points).ToMyVector3();
            Normalizer3 normalizer = new Normalizer3(new List<MyVector3>(pointsSet));
            HashSet<MyVector3> normalizedPoints = normalizer.Normalize(pointsSet);

            Log($"Generating points for {stopWatch.ElapsedMilliseconds / 1000f} seconds.");

            stopWatch.Restart();
            // Generate a convex hull which will serve as a 3D Delaunay Triangulation of the fibionacci points.
            convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(normalizedPoints, false, removeInnerPoints: removeInnerPoints);

            Log($"Generating convex hull for {stopWatch.ElapsedMilliseconds / 1000f} seconds.");
            stopWatch.Restart();

            // Set and denormalize voronoi cells.
            voronoiCells = Delaunay3DToVoronoiAlgorithm.GenerateVoronoiDiagram(convexHull, center: centerType);
            Log($"Generating Voronoi diagram for {stopWatch.ElapsedMilliseconds / 1000f}");
            stopWatch.Restart();

            voronoiCells = normalizer.UnNormalize(voronoiCells);
            Log($"Denormalizing voronoiCells for {stopWatch.ElapsedMilliseconds / 1000f}.");
            stopWatch.Stop();
            convexHull = normalizer.UnNormalize(convexHull);
        }


        private void AssignEdges()
        {
            //// Get vertices from convex hull in an array. Should be equal to fibionacci points.
            //HalfEdgeVertex3[] convexHullVertices = new HalfEdgeVertex3[convexHull.verts.Count];
            //convexHull.verts.CopyTo(convexHullVertices);

            //// Get voronoi cells in an array.
            //VoronoiCell3[] voronoiCellArray = new VoronoiCell3[voronoiCells.Count];
            //voronoiCells.CopyTo(voronoiCellArray);

            // The voronoi cells should be in the same
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            foreach (VoronoiCell3 cell in voronoiCells)
            {
                Gizmos.DrawSphere(transform.rotation * cell.sitePos.ToVector3(), 0.01f);
                foreach (VoronoiEdge3 edge in cell.edges)
                {
                    Vector3 p1 = transform.rotation * edge.p1.ToVector3();
                    Vector3 p2 = transform.rotation * edge.p2.ToVector3();

                    Gizmos.DrawLine(p1, p2);
                }
            }

            //Gizmos.color = Color.blue;

            //foreach (Vector3 point in points)
            //{
            //    Gizmos.DrawSphere(transform.rotation * point, 0.01f);
            //}

            //Gizmos.color = Color.red;
            //int i = 0;

            //foreach (HalfEdgeVertex3 vert in convexHull.verts)
            //{
            //    i++;

            //    Gizmos.DrawWireSphere(vert.position.ToVector3(), 0.1f);

            //    //HashSet<HalfEdge3> edges = vert.GetEdgesPointingToVertex(convexHull);
            //    //foreach(HalfEdge3 edge in edges)
            //    //{
            //    //    Vector3 p1 = transform.rotation * edge.v.position.ToVector3();
            //    //    Vector3 p2 = transform.rotation * edge.nextEdgew.v.position.ToVector3();

            //    //    Gizmos.DrawLine(p1, p2);

            //    //}
            //    if (i == 24)
            //        break;
            //}
            //int j = 0;



            //Gizmos.color = Color.magenta;

            //foreach (VoronoiCell3 cell in voronoiCells)
            //{
            //    j++;
            //    Gizmos.DrawSphere(cell.sitePos.ToVector3(), 0.05f);
            //    if (j == 20)
            //        break;
            //}
        }
    }
}

