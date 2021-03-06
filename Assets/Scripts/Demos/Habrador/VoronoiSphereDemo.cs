using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiSphereDemo : MonoBehaviour
{
    public int seed = 300;
    public int n = 8;
    public float radius = 8;
    private HashSet<MyVector3> points;
    private HashSet<VoronoiCell3> voronoiCells;
    [System.NonSerialized]
    public List<VoronoiCell3> voronoiCellsList;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    points = GenerateRandomPointsOnSphere(n, radius);
    //}

    HashSet<MyVector3> GenerateRandomPointsOnSphere(int n, float radius)
    {
        HashSet<MyVector3> points = new HashSet<MyVector3>();

        for(int i=0; i < n; i++)
        {
            Vector3 point = Random.onUnitSphere * radius;
            points.Add(point.ToMyVector3());
        }

        return points;
    }
    
    private void GenerateVoronoiDiagram(HashSet<MyVector3> points)
    {
        HalfEdgeData3 convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(points, true);
        voronoiCells = Delaunay3DToVoronoiAlgorithm.GenerateVoronoiDiagram(convexHull);
        voronoiCellsList = new List<VoronoiCell3>(voronoiCells);
    }

    //private void GenerateConvexHullMesh(HashSet<MyVector3> points)
    //{
    //    HalfEdgeData3 convexHull = IterativeHullAlgorithm3D.GenerateConvexHull(points, true);
    //    Mesh mesh = convexHull.ConvertToMyMesh("Planet", MyMesh.MeshStyle.HardAndSoftEdges).ConvertToUnityMesh(true, "Planet");

    //    MeshFilter meshFilter = GetComponent<MeshFilter>();
    //    if(meshFilter)
    //    {
    //        meshFilter.mesh = mesh;
    //    }
    //}

    private void OnValidate()
    {
        Random.InitState(seed);
        points = GenerateRandomPointsOnSphere(n, radius);
        GenerateVoronoiDiagram(points);
    }

    //private void OnDrawGizmos()
    //{
    //    Random.InitState(seed);
    //    //Gizmos.color = Color.blue;
    //    foreach(VoronoiCell3 cell in voronoiCellsList)
    //    {
    //        Gizmos.color = Random.ColorHSV(0, 1);

    //        Vector3 drawPoint = cell.sitePos.ToVector3() + transform.position;
    //        drawPoint = transform.rotation * drawPoint;
    //        Gizmos.DrawSphere(drawPoint, 0.1f);

    //        foreach (VoronoiEdge3 edge in cell.edges)
    //        {
    //            Vector3 p1 = edge.p1.ToVector3() + transform.position;
    //            Vector3 p2 = edge.p2.ToVector3() + transform.position;

    //            Vector3 drawP1 = transform.rotation * p1;
    //            Vector3 drawP2 = transform.rotation * p2;

    //            //Gizmos.DrawLine(drawP1, drawP2);



    //            Vector3 p1OnSphere = transform.position + (radius * p1.normalized);
    //            Vector3 p2OnSphere = transform.position + (radius * p2.normalized);
    //            Vector3 drawP1OnSphere = transform.rotation * p1OnSphere;
    //            Vector3 drawP2OnSphere = transform.rotation * p2OnSphere;

    //            //Gizmos.DrawCube(drawP1OnSphere, Vector3.one * 0.1f);
    //            //Gizmos.DrawCube(drawP2OnSphere, Vector3.one * 0.1f);

    //            Gizmos.DrawLine(drawP1OnSphere, drawP2OnSphere);

    //        }
    //    }
    //}
}
