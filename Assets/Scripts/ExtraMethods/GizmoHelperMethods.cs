
using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    public static class GizmoHelperMethods
    {
        public static void DrawVoronoiTiles(Vector3 pos, Quaternion rot, float radius, HashSet<VoronoiTile> voronoiTiles)
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

        public static void HighlightTileNeighbors(Vector3 pos, Quaternion rot, float radius, HashSet<VoronoiTile> voronoiTiles)
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

        public static void DrawVoronoiCells(Vector3 pos, Quaternion rot, float radius, HashSet<VoronoiCell3> voronoiCells)
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
