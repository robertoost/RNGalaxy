using Habrador_Computational_Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy 
{
    public class Plate
    {
        public Vector3 center;
        public List<PlateEdge> edges;
        
        // Continental or oceanic.
        public PlateType plateType;

        // All vertices in the sphere mesh belonging to this plate.
        public List<int> vertices = new List<int>();

        public enum PlateType
        {
            oceanic = 0,
            continental = 1
        }

        public Plate(Vector3 center, PlateType plateType, List<VoronoiEdge3> voronoiEdges)
        {

            this.center = center;
            this.plateType = plateType;
            edges = new List<PlateEdge>();

            // Set edges.
            foreach (VoronoiEdge3 voronoiEdge in voronoiEdges)
            {
                PlateEdge plateEdge = new PlateEdge(voronoiEdge.p1.ToVector3(), voronoiEdge.p2.ToVector3(), this);
                edges.Add(plateEdge);
            }
        }
    }
}