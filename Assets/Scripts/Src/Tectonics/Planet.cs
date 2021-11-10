using Habrador_Computational_Geometry;
using MIConvexHull;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RNGalaxy.TectonicPlate;



namespace RNGalaxy
{


    [System.Serializable]
    public class Planet
    {
        public int randomSeed = 42;
        
        [Range(100, 10000)]
        public int numPoints = 100;

        [Range(6f, 8f)]
        public float radius = 8f;

        [Range(1, 200)]
        public int numPlates = 30;
        
        [Range(0, 1)]
        public float jitter = 0f;

        const float ELEVATION_MIN = 1f;
        const float ELEVATION_MAX = 1.5f;

        private ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull;
        [System.NonSerialized]
        public HashSet<VoronoiTile> voronoiTiles;
        public List<TectonicPlate> plates;
        [System.NonSerialized]
        public Vector3[] points;
        public Planet(int numPoints, float radius, float jitter, int randomSeed = 42)
        {
            this.numPoints = numPoints;
            this.radius = radius;
            this.jitter = jitter;
            this.randomSeed = randomSeed;
        }

        public void GenerateTectonicPlates()
        {
            Random.InitState(randomSeed);
            // Generate convex hull and voronoi tiles.
            HalfEdgeData3 convexHull = CreateConvexHull();
            voronoiTiles = CreateVoronoiTiles(convexHull);

            // Run flood fill to generate continents.
            plates = CreateTectonicPlates(voronoiTiles);
            
        }

        private HalfEdgeData3 CreateConvexHull()
        {
            // Convert points to a list of arrays containing doubles.
            points = FibionacciSphere.GeneratePoints(numPoints, radius, jitter);
            List<double[]> convertedPoints = points.ToDoubleArrayList();

            // Generate a quick convex hull.
            convexHull = ConvexHull.Create(convertedPoints).Result;

            // Generate half edge data from the convex hull for use in the voronoi diagram.
            HalfEdgeData3 halfEdgeData = HalfEdgeMapper.MapHalfEdgeData(convexHull);
            return halfEdgeData;
        }

        private HashSet<VoronoiTile> CreateVoronoiTiles(HalfEdgeData3 convexHull)
        {
            // Generate voronoi tiles with triangle centroids.
            var center = Delaunay3DToVoronoiTile.VoronoiCenter.Centroid;
            voronoiTiles = Delaunay3DToVoronoiTile.GenerateVoronoiDiagram(convexHull, center: center);
            return voronoiTiles;
        }

        /// <summary>
        /// Generates tectonic plates as collections of voronoi tiles by random flood filling.
        /// </summary>
        /// <param name="voronoiTiles">The list of tiles to turn into plates.</param>
        /// <returns>A list of tectonic plates.</returns>
        private List<TectonicPlate> CreateTectonicPlates(HashSet<VoronoiTile> voronoiTiles) {

            // TODO: Change this to be two separate function calls, that divide big and small plates.
            List<TectonicPlate> plates = FloodFillPlates(voronoiTiles);

            SetPlateTypes(plates);
            SetTileElevations(plates);
            //SetEdgeDistanceValues(plates);
            return plates;


        }


        //// Divide plates amounts.
        // Earth has
        //int smallToBigPlateRatio = 3;
        //int quotient = numPlates / smallToBigPlateRatio;
        //int remainder = numPlates % smallToBigPlateRatio;

        // 95% Of the earth's surface is covered by big plates.
        //int numTiles = voronoiTiles.Count;
        //int bigToSmallPlateTileRatio = 20
        //int tileQuotient = 

        //int numBigPlates = quotient;
        //int numSmallPlates = (smallToBigRatio - 1) * quotient + remainder;

        // Convert tile hashset to list.
        // TODO: Alter floodfill plates to take numTiles and numPlates parameters.
        // Call floodfill plates with the initial tile list.

        // Call second floodfill plates with the reduced tile list and remainder of plates.
        private List<TectonicPlate> FloodFillPlates(HashSet<VoronoiTile> voronoiTiles)
        {

            // Create lists for keeping track of our progress.
            List<TectonicPlate> tectonicPlates = new List<TectonicPlate>();
            List<List<VoronoiTile>> tilesToExpand = new List<List<VoronoiTile>>();
            List<VoronoiTile> removedTiles = new List<VoronoiTile>();

            // Pick a starting tile for every plate.
            for (int i = 0; i < numPlates; i++)
            {
                VoronoiTile tile = voronoiTiles.ElementAt(Random.Range(0, voronoiTiles.Count));
                TectonicPlate plate = new TectonicPlate(tile);
                tectonicPlates.Add(plate);

                // Prepare a list of expansion queues.
                tilesToExpand.Add(new List<VoronoiTile>() { tile });

                // Prevent random selection of this tile in the next loop
                voronoiTiles.Remove(tile);
                removedTiles.Add(tile);
            }

            // Re-add each removed tile so the list stays unaltered.
            foreach (VoronoiTile removedTile in removedTiles)
            {
                voronoiTiles.Add(removedTile);
            }

            List<TectonicPlate> finishedPlates = new List<TectonicPlate>();

            // Add each tile to a tectonic plate.
            // Continue while there's still tiles left.
            int safety = 0;

            while (finishedPlates.Count < numPlates)
            {
                int plateId = Random.Range(0, tectonicPlates.Count);
                TectonicPlate tectonicPlate = tectonicPlates[plateId];
                List<VoronoiTile> expansionQueue = tilesToExpand[plateId];

                // Get the tile that we'll expand on, and remove it from the expansion queue.
                int expansionTileId = Random.Range(0, expansionQueue.Count);
                VoronoiTile expandTile = expansionQueue[expansionTileId];
                expansionQueue.RemoveAt(expansionTileId);

                // For each edge of this tile, try to add the neighboring tile to the plate.
                foreach (VoronoiTileEdge expandTileEdge in expandTile.edges)
                {

                    // If the neighboring tile is already taken by a plate, continue to the next tile.
                    VoronoiTile neighborTile = expandTileEdge.oppositeTile;
                    if (neighborTile.plate != null)
                    {
                        if (neighborTile.plate != tectonicPlate)
                        {
                            expandTile.distanceToEdge = -1;
                        }
                        continue;
                    }
                    
                    // Add this tile to the current plate. 
                    tectonicPlate.plateTiles.Add(neighborTile);
                    neighborTile.plate = tectonicPlate;
                    
                    // Then add the tile to the expansion queue.
                    expansionQueue.Add(neighborTile);
                }

                // If the expansion queue is empty, this plate can no longer expand.
                if (expansionQueue.Count == 0)
                {
                    tectonicPlates.RemoveAt(plateId);
                    tilesToExpand.RemoveAt(plateId);
                    finishedPlates.Add(tectonicPlate);

                }

                safety++;
                if (safety > 100000)
                {
                    Debug.Log("Infinite loop detected while generating plates.");
                }
            }
            return finishedPlates;
        }

        // TODO: Ideally, we set the plate type and elevation values while we're already creating the plates, and adding the tiles to them.
        // That way, we don't have to loop over everything again.
        private void SetPlateTypes(List<TectonicPlate> plates)
        {
            foreach (TectonicPlate plate in plates)
            {
                plate.plateType = Random.Range(0f, 1f) > 0.5 ? PlateType.Continental : PlateType.Oceanic;
            }
        }

        private void SetTileElevations(List<TectonicPlate> plates)
        {
            Dictionary<TectonicPlate, List<bool>> plateCollisions = new Dictionary<TectonicPlate, List<bool>>();

            for (int i=0; i < plates.Count; i++)
            {
                TectonicPlate plate = plates[i];
                List<bool> collisionList = new List<bool>();
                plateCollisions[plate] = collisionList;

                foreach (TectonicPlate otherPlate in plates)
                {
                    bool collision;
                    if (plate.plateType != otherPlate.plateType)
                    {
                        collision = Random.Range(0, 1) > 0.9f;
                    } else
                    {
                        collision = Random.Range(0, 1) > 0.4f;
                    }
                    collisionList.Add(collision);
                }
            }

            for (int i = 0; i < plates.Count; i++)
            {
                TectonicPlate plate = plates[i];

                foreach (VoronoiTile tile in plate.plateTiles)
                {
                    int sign = plate.plateType == PlateType.Continental ? 1 : -3;
                    float elevation = sign * Random.Range(ELEVATION_MIN, ELEVATION_MAX);

                    tile.elevation = elevation;

                    // We've found a border tile.
                    if (tile.distanceToEdge == -1 && plate.plateType == PlateType.Continental)
                    {
                        VoronoiTileEdge edge = tile.edges.First(edge => edge.oppositeTile.plate != plate);

                        bool collision = plateCollisions[edge.oppositeTile.plate][i];

                        //if (DetectCollision(otherEdge.tile.plate, plate))
                        if (collision)
                        {
                            tile.elevation = 8f;
                        }
                    }
                }
            }
        }

        

        // TODO: True collision detection.
        // Dummy method. Should use actual geoditical movement, but uses basic 2d directions instead.
        // Is messy, but works.
        //private bool DetectCollision(TectonicPlate plateA, TectonicPlate plateB)
        //{
        //    // If the direction vectors form an acute angle, detect a collision.
        //    return Vector2.Dot(plateA.direction, plateB.direction) > 0;
        //}

        //private void SetTileElevation(int distanceToEdge, VoronoiTile tile)
        //{

        //}
    }
}
