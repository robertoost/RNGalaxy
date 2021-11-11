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
        public Planet planet = new Planet(100, 8, 0.5f);

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

        public void UpdatePlanet(int randomSeed, int numPoints, float radius, float tileJitter, float mountainElevation, float roughness, float baseLandHeight)
        {
            planet = new Planet(numPoints, radius, tileJitter, randomSeed: randomSeed);
            mountainAmplitude = mountainElevation;
            roughnessAmplitude = roughness;
            this.baseLandHeight = baseLandHeight;
            UpdatePlanet();
        }

        private void UpdatePlanet() {
            // Also initiates the random state.
            planet.GenerateTectonicPlates();

            landColor = Random.ColorHSV(0, 1, 0.75f, 1f);
            waterColor = Random.ColorHSV(0, 1, 0.75f, 1f);
            landMeshRenderer.material.color = landColor;
            waterMeshRenderer.material.color = waterColor;

            if (!planetMeshFilter)
            {
                planetMeshFilter = GetComponentInChildren<MeshFilter>();
            }

            if (generatedMesh)
            {
                Destroy(generatedMesh);
            }
            generatedMesh = Instantiate(planetMesh);

            planetMeshFilter.mesh = generatedMesh;

            transform.localScale = Vector3.one * 2 * planet.radius;

            SampleMesh(planetMesh);
            RaiseVertices();

        }

        // TODO: Improve vertex sampling.
        // This sampling method is inaccurate. In practice, we're sampling the circumcenter voronoi diagram,
        // while we actually want to check the centroid diagram.
        private void SampleMesh(Mesh mesh)
        {
            List<VoronoiTile> tiles = new List<VoronoiTile>(planet.voronoiTiles);

            int numTiles = tiles.Count;
            Vector3[] tileCenters = new Vector3[numTiles];
            for(int i=0; i < numTiles; i++)
            {
                tileCenters[i] = tiles[i].sitePos.ToVector3();
            }

            KDTree kdTree = KDTree.MakeFromPoints(tileCenters);

            Vector3[] vertices = planetMeshFilter.sharedMesh.vertices;
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
            Debug.Log("Raising vertices!");
            Vector3[] vertices = planetMeshFilter.mesh.vertices;
            foreach (VoronoiTile tile in planet.voronoiTiles)
            {
                foreach (int vertexID in tile.vertices)
                {
                    Vector3 vertex = vertices[vertexID];
                    Vector3 normalizedVertex = vertex.normalized;

                    int sign = tile.plate.plateType == TectonicPlate.PlateType.Continental ? 1 : -2;
                    Vector3 elevation = 0.1f * baseLandHeight * vertex.normalized * sign;

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
            planetMeshFilter.mesh.vertices = vertices;
            planetMeshFilter.mesh.RecalculateNormals();
        }
    }

}
