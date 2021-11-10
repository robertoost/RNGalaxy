using Habrador_Computational_Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy
{
    /// <summary>
    /// Code partially copied from Habrador's Computational Geometry library. https://github.com/Habrador/Computational-geometry
    /// Heavy alterations have made it possible to store neighboring locations.
    /// </summary>
    public static class Delaunay3DToVoronoiTile
    {
        public enum VoronoiCenter
        {
            Circumcenter = 0,
            Centroid = 1
        }

        //Generate a Voronoi diagram in 3d space given a Delaunay triangulation in 3d space
        public static HashSet<VoronoiTile> GenerateVoronoiDiagram(HalfEdgeData3 delaunayTriangulation, VoronoiCenter center = VoronoiCenter.Circumcenter)
        {
            //If we dont need the voronoi sitePos, which is the center of the voronoi cell, we can use the half-edge data structure
            //If not we have the create a child class for voronoi
            HashSet<VoronoiTile> voronoiDiagram = new HashSet<VoronoiTile>();
            List<Tuple<VoronoiTile, HalfEdgeVertex3>> voronoiTiles = new List<Tuple<VoronoiTile, HalfEdgeVertex3>>();

            //Step 1. Generate a center of circle for each triangle because this process is slow in 3d space
            Dictionary<HalfEdgeFace3, MyVector3> triangleCenterLookup = new Dictionary<HalfEdgeFace3, MyVector3>();

            HashSet<HalfEdgeFace3> delaunayTriangles = delaunayTriangulation.faces;

            foreach (HalfEdgeFace3 triangle in delaunayTriangles)
            {
                MyVector3 p1 = triangle.edge.v.position;
                MyVector3 p2 = triangle.edge.nextEdge.v.position;
                MyVector3 p3 = triangle.edge.nextEdge.nextEdge.v.position;

                MyVector3 centerPoint = center == VoronoiCenter.Circumcenter ? _Geometry.CalculateCircleCenter(p1, p2, p3) : _Geometry.CalculateTriangleCenter(p1, p2, p3);

                triangleCenterLookup.Add(triangle, centerPoint);
            }

            //Step 2. Generate the voronoi cells
            HashSet<HalfEdgeVertex3> delaunayVertices = delaunayTriangulation.verts;

            //In the half-edge data structure we have multiple vertices at the same position, 
            //so we have to track which vertex positions have been added
            //HashSet<MyVector3> addedSites = new HashSet<MyVector3>();
            Dictionary<MyVector3, VoronoiTile> addedSites = new Dictionary<MyVector3, VoronoiTile>();

            // Make sure to add each site.
            foreach (HalfEdgeVertex3 v in delaunayVertices)
            {
                //Has this site already been added?
                if (addedSites.ContainsKey(v.position))
                {
                    continue;
                }

                //This vertex is a site pos in the voronoi diagram
                VoronoiTile tile = new VoronoiTile(v.position);
                addedSites.Add(v.position, tile);

                voronoiDiagram.Add(tile);
                voronoiTiles.Add(new Tuple<VoronoiTile, HalfEdgeVertex3>(tile, v));
            }

            foreach (Tuple<VoronoiTile, HalfEdgeVertex3> tileCellTuple in voronoiTiles)
            {
                //All triangles are fully connected so no null opposite edges should exist
                //So to generate the voronoi cell, we just rotate clock-wise around each vertex in the delaunay triangulation
                VoronoiTile tile = tileCellTuple.Item1;
                HalfEdgeVertex3 v = tileCellTuple.Item2;
                HalfEdge3 currentEdge = v.edge;
            
                int safety = 0;

                while (true)
                {
                    //Build an edge going from the opposite face to this face
                    //Each vertex has an edge going FROM it
                    HalfEdgeFace3 oppositeTriangle = currentEdge.oppositeEdge.face;

                    HalfEdgeFace3 thisTriangle = currentEdge.face;

                    MyVector3 oppositeTriangleCenter = triangleCenterLookup[oppositeTriangle];

                    MyVector3 thisTriangleCenter = triangleCenterLookup[thisTriangle];

                    VoronoiTileEdge edge = new VoronoiTileEdge(oppositeTriangleCenter, thisTriangleCenter, v.position);
                    edge.tile = tile;
                    tile.edges.Add(edge);

                    //Jump to the next triangle
                    //Each vertex has an edge going FROM it
                    //And we want to rotate around a vertex clockwise
                    //So the edge we should jump over is:
                    HalfEdge3 jumpEdge = currentEdge.nextEdge.nextEdge;

                    HalfEdge3 oppositeEdge = jumpEdge.oppositeEdge;

                    //Are we back where we started?
                    if (oppositeEdge == v.edge)
                    {
                        break;
                    }

                    currentEdge = oppositeEdge;

                    safety += 1;

                    if (safety > 10000)
                    {
                        throw new System.Exception("Stuck in infinite loop when generating voronoi cells");
                    }
                }
            }

            foreach (Tuple<VoronoiTile, HalfEdgeVertex3> tileCellTuple in voronoiTiles)
            {
                //All triangles are fully connected so no null opposite edges should exist
                //So to generate the voronoi cell, we just rotate clock-wise around each vertex in the delaunay triangulation
                VoronoiTile tile = tileCellTuple.Item1;
                HalfEdgeVertex3 v = tileCellTuple.Item2;
                HalfEdge3 currentEdge = v.edge;


                foreach (VoronoiTileEdge edge in tile.edges)
                {
                    // Get the center of the neighboring tile through the current edge.
                    // This center can then be used to look up the neighbor tile object.
                    MyVector3 neighborTileCenter = currentEdge.nextEdge.v.position;
                    VoronoiTile neighborTile = addedSites[neighborTileCenter];

                    edge.oppositeTile = neighborTile;

                    // Safety no longer needed, as the previous infinite loop should have been avoided.
                    HalfEdge3 jumpEdge = currentEdge.nextEdge.nextEdge;
                    currentEdge = jumpEdge.oppositeEdge;
                }
            }

            return voronoiDiagram;
        }
    }
}
