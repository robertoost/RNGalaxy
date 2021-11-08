using Habrador_Computational_Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNGalaxy 
{
    public class PlateEdge
    {
        public Vector3 start;
        public Vector3 end;

        // The two plates that share this edge.
        public Plate[] parents = new Plate[2];

        public PlateEdge(Vector3 start, Vector3 end, Plate parent)
        {
            this.start = start;
            this.end = end;
            parents[0] = parent;
        }
    }
}