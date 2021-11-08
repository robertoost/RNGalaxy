using Habrador_Computational_Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNGalaxy
{
    public class VoronoiTile
    {  
        //All positions within a voronoi cell is closer to this position than any other position in the diagram
        //Is also a vertex in the delaunay triangulation
        public MyVector3 sitePos;

        public List<VoronoiTileEdge> edges = new List<VoronoiTileEdge>();

        public VoronoiTile(MyVector3 sitePos)
        {
            this.sitePos = sitePos;
        }


    }

    public class VoronoiTileEdge
    {
        //These are the voronoi vertices
        public MyVector3 p1;
        public MyVector3 p2;

        //All positions within a voronoi cell is closer to this position than any other position in the diagram
        //Is also a vertex in the delaunay triangulation
        public MyVector3 sitePos;
        public VoronoiTile tile;
        public VoronoiTile oppositeTile;

        public VoronoiTileEdge(MyVector3 p1, MyVector3 p2, MyVector3 sitePos)
        {
            this.p1 = p1;
            this.p2 = p2;

            this.sitePos = sitePos;
        }
    }

}
