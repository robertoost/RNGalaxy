using Habrador_Computational_Geometry;
using Noise;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RNGalaxy
{
    public class PlanetGenerator : MonoBehaviour
    {
        public Planet planet = new Planet(200, 8, 0.5f);

        public Mesh planetMesh;

        private MeshFilter planetMeshFilter;

        private Mesh generatedMesh;

        public bool updateOnValidate = false;

        [Range(0.1f, 2f)]
        public float baseLandHeight = 1f;

        [Range(0f, 1f)]
        public float mountainAmplitude = 1f;

        [Range(0f, 1f)]
        public float roughnessAmplitude = 1f;

        private Color landColor;
        private Color waterColor;
        public MeshRenderer landMeshRenderer;
        public MeshRenderer waterMeshRenderer;

        private void OnValidate()
        {
            if (!Application.isPlaying || !updateOnValidate)
            {
                return;
            }
            UpdatePlanet();
        }

        // Method that the GUI can access.
        public void UpdatePlanet(int randomSeed, int numPoints, int numPlates, float radius, float tileJitter, float mountainElevation, float roughness, float baseLandHeight)
        {
            planet = new Planet(numPoints, radius, tileJitter, randomSeed: randomSeed, numPlates:numPlates);
            mountainAmplitude = mountainElevation;
            roughnessAmplitude = roughness;
            this.baseLandHeight = baseLandHeight;
            UpdatePlanet();
        }

        // Updates the planet.
        private void UpdatePlanet() {
            // Initiate random state to get the random colors before anything else changes.
            Random.InitState(planet.randomSeed);
            landColor = Random.ColorHSV(0, 1, 0.75f, 1f);
            waterColor = Random.ColorHSV(0, 1, 0.75f, 1f);

            // Change land and water colors.
            landMeshRenderer.material.color = landColor;
            waterMeshRenderer.material.color = waterColor;
            
            // Generate tectonic plates with elevation, collisions, etc.
            planet.GenerateTectonicPlates();

            if (!planetMeshFilter)
            {
                planetMeshFilter = GetComponentInChildren<MeshFilter>();
            }

            // Destroy the previous planet.
            if (generatedMesh)
            {
                Destroy(generatedMesh);
            }

            // Instantiate a new sphere.
            generatedMesh = Instantiate(planetMesh);
            planetMeshFilter.mesh = generatedMesh;

            // Set the scale to the radius.
            transform.localScale = Vector3.one * 2 * planet.radius;

            // Sample the mesh, then raise the vertices.
            SampleMesh(planetMesh);
            RaiseVertices();
        }

        // TODO: Improve vertex sampling.
        // This sampling method is inaccurate. In practice, we're sampling the circumcenter voronoi diagram,
        // while we actually want to check the centroid diagram.
        private void SampleMesh(Mesh mesh)
        {
            List<VoronoiTile> tiles = new List<VoronoiTile>(planet.voronoiTiles);

            // Get a list of tile centers to add to the KD tree.
            int numTiles = tiles.Count;
            Vector3[] tileCenters = new Vector3[numTiles];
            for(int i=0; i < numTiles; i++)
            {
                tileCenters[i] = tiles[i].sitePos.ToVector3();
            }

            // Use a KD tree to make sampling less hard our CPU.
            KDTree kdTree = KDTree.MakeFromPoints(tileCenters);

            Vector3[] vertices = planetMeshFilter.sharedMesh.vertices;

            // For every vertex in the original mesh, find the nearest tile. Then add the vertex id to that tile.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                int nearestTileID = kdTree.FindNearest(vertex);
                VoronoiTile nearestTile = tiles[nearestTileID];
                nearestTile.vertices.Add(i);
            }
        }

        private void RaiseVertices()
        {
            Vector3[] vertices = planetMeshFilter.mesh.vertices;
            foreach (VoronoiTile tile in planet.voronoiTiles)
            {
                foreach (int vertexID in tile.vertices)
                {
                    Vector3 vertex = vertices[vertexID];
                    Vector3 normalizedVertex = vertex.normalized;

                    // Generate oceans and land by setting the base elevation.
                    int sign = tile.plate.plateType == TectonicPlate.PlateType.Continental ? 1 : -2;
                    Vector3 elevation = 0.1f * baseLandHeight * vertex.normalized * sign;

                    // Apply noise using the elevation value, which will be higher if there's mountains on the tile.
                    if (tile.plate.plateType == TectonicPlate.PlateType.Continental)
                    {
                        float perlinNoise = Mathf.Abs(Perlin.Noise(normalizedVertex * planet.radius));
                        float perlinNoiseB = Mathf.Abs(Perlin.Noise(normalizedVertex * 10 * planet.radius));
                        Vector3 mountains = tile.elevation * vertex.normalized * perlinNoise * 0.5f;
                        Vector3 roughness = vertex.normalized * perlinNoiseB * 0.1f;
                        elevation += roughness * roughnessAmplitude + mountains * mountainAmplitude;
                    }

                    vertices[vertexID] = vertex + elevation;
                }
            }

            // Update mesh.
            planetMeshFilter.mesh.vertices = vertices;
            planetMeshFilter.mesh.RecalculateBounds();
            planetMeshFilter.mesh.RecalculateNormals();
        }
    }

}
