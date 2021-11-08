using Habrador_Computational_Geometry;
using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RNGalaxy
{
    class HalfEdgeMapper
    {
        /// <summary>
        /// Maps the default convex hull result from MIConvexHull to a datastructure fit for 3D voronoi diagrams.
        /// </summary>
        /// <param name="convexHull"></param>
        /// <returns></returns>
        public static HalfEdgeData3 MapHalfEdgeData(ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull)
        {
            // Only the faces are needed. The vertices are already embedded in the faces.
            IEnumerable<DefaultConvexFace<DefaultVertex>> faces = convexHull.Faces;

            // Generate half edge faces from the convex hull faces.
            List<HalfEdgeFace3> halfEdgeFaceList = ToHalfEdgeFace3(faces);

            // Convert to a hashset for the built in "half edge face -> half edge data" method.
            HashSet<HalfEdgeFace3> halfEdgeFaces = new HashSet<HalfEdgeFace3>(halfEdgeFaceList);
            HalfEdgeData3 halfEdgeData = HalfEdgeData3.GenerateHalfEdgeDataFromFaces(halfEdgeFaces);

            return halfEdgeData;
        }


        public static List<HalfEdgeFace3> ToHalfEdgeFace3(IEnumerable<DefaultConvexFace<DefaultVertex>> faces)
        {

            List<HalfEdgeFace3> halfEdgeFaces = new List<HalfEdgeFace3>();
            var faceDictionary = new Dictionary<DefaultConvexFace<DefaultVertex>, int>();

            int idx = 0;
            foreach (var face in faces)
            {
                // Store face for easy lookup.
                faceDictionary[face] = idx;

                // Create face using the first halfEdge.
                halfEdgeFaces.Add(MapHalfEdgeFace(face));

                idx++;
            }

            idx = 0;
            foreach (var face in faces)
            {
                var adjacentFaces = face.Adjacency;
                HalfEdgeFace3 halfEdgeFace = halfEdgeFaces[idx];

                // We start with the second edge.
                HalfEdge3 currentEdge = halfEdgeFace.edge.nextEdge;

                for (int i = 0; i < 3; i++)
                {
                    // From the MIConvexHull DefaultConvexFace Adjacency documentation:
                    // If F = Adjacency[i] then the vertices shared with F are Vertices[j] where j != i.
                    //
                    // Current adjacent face is i, so adjacent edges are j -> k.
                    var adjacentFace = adjacentFaces[i];

                    // Get the neighbor halfEdgeFace through the face dictionary.
                    int halfEdgeFaceId = faceDictionary[adjacentFace];
                    HalfEdgeFace3 neighborFace = halfEdgeFaces[halfEdgeFaceId];

                    // Neighbor's adjacent faces.
                    var neighborAdjacency = adjacentFace.Adjacency;
                    int neighborAdjacentIdx = 0;

                    // Infer the adjacency index for the neighboring face.
                    foreach (var neighborAdjacentFace in neighborAdjacency)
                    {

                        // If the current neighbor's neighbor is me, we've found our neighbor index.
                        if (face == neighborAdjacentFace)
                        {
                            // Add 1 to the neighbor index and break.
                            neighborAdjacentIdx = (neighborAdjacentIdx + 1) % 3;
                            break;
                        }
                        neighborAdjacentIdx++;
                    }

                    // Get each of the neighbor's edges.
                    HalfEdge3 neighborEdge1 = neighborFace.edge; // 0 -> 1
                    HalfEdge3 neighborEdge2 = neighborEdge1.nextEdge; // 1 -> 2
                    HalfEdge3 neighborEdge3 = neighborEdge2.nextEdge; // 2 -> 3

                    // Prepare a list for using the index on.
                    HalfEdge3[] neighborEdges = new HalfEdge3[] { neighborEdge1, neighborEdge2, neighborEdge3 };

                    // The opposite edge must contain j -> k. These are half edges, so we get half edge j.
                    // In this case, it's the neighbor's adjacency index that matches the current face, + 1.
                    HalfEdge3 oppositeEdge = neighborEdges[neighborAdjacentIdx];

                    currentEdge.oppositeEdge = oppositeEdge;
                    oppositeEdge.oppositeEdge = currentEdge;

                    // Move to next edge.
                    currentEdge = currentEdge.nextEdge;
                }
                idx++;
            }
            return halfEdgeFaces;
        }

        public static HalfEdgeFace3 MapHalfEdgeFace(DefaultConvexFace<DefaultVertex> face)
        {
            MyVector3 normal = face.Normal.ToMyVector3();

            // Make HalfEdgeVertices and edges for every face.
            HalfEdgeVertex3[] halfEdgevertices = new HalfEdgeVertex3[3];
            HalfEdge3[] halfEdges = new HalfEdge3[3];

            // Convert every face to halfEdges.
            for (int i = 0; i < face.Vertices.Length; i++)
            {
                // Get face vertex position.
                DefaultVertex defaultVertex = face.Vertices[i];
                MyVector3 vertex = defaultVertex.ToMyVector3();

                // Convert vertex to half edge vertex.
                HalfEdgeVertex3 halfEdgeVertex = new HalfEdgeVertex3(vertex, normal);
                halfEdgevertices[i] = halfEdgeVertex;

                // Create half edge starting with the vertex position.
                HalfEdge3 halfEdge = new HalfEdge3(halfEdgeVertex);
                halfEdges[i] = halfEdge;

                // Link vertex to edge.
                halfEdgeVertex.edge = halfEdge;
            }

            // Create face using the first halfEdge.
            HalfEdgeFace3 halfEdgeFace = new HalfEdgeFace3(halfEdges[0]);

            for (int i = 0, j = 1, k = 2; i < 3; i++, j++, k++, j %= 3, k %= 3)
            {
                // Get the previous, current, and next edges.
                HalfEdge3 currentEdge = halfEdges[i];
                HalfEdge3 nextEdge = halfEdges[j];
                HalfEdge3 prevEdge = halfEdges[k];

                // Link the other edges to the currentEdge, and the current edge to the face.
                currentEdge.prevEdge = prevEdge;
                currentEdge.nextEdge = nextEdge;
                currentEdge.face = halfEdgeFace;
            }

            return halfEdgeFace;
        }
    }
}
