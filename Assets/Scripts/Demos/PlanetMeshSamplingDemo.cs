using Habrador_Computational_Geometry;
using Noise;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RNGalaxy
{
    public class PlanetMeshSamplingDemo : MonoBehaviour
    {
        public Planet planet = new Planet(100, 8, 0.5f);

        public Mesh planetMesh;

        private MeshFilter planetMeshFilter;

        private Mesh generatedMesh;

        [Range(0f, 1f)]
        public float elevationAmplification = 1f;

        public float amplitude = 1f;

        private float perlinMin = -Mathf.Sqrt(3) / 2;
        private float perlinMax = Mathf.Sqrt(3) / 2;

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            planet.GenerateTectonicPlates();

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

                    Vector3 elevation = elevationAmplification * (tile.elevation * normalizedVertex);

                    if (tile.plate.plateType == TectonicPlate.PlateType.Continental)
                    {
                        float perlinNoise = Perlin.Noise(vertex);
                        float mountainHeight = Mathf.InverseLerp(perlinMin, perlinMax, perlinNoise);
                        //if (mountainHeight == 0)
                        //{
                        //    throw new System.Exception();
                        //}
                        float roughness = amplitude * 1f;
                        elevation += (vertex.normalized * mountainHeight * roughness);
                    }

                    vertices[vertexID] = vertex + elevation;
                }
            }
            planetMeshFilter.mesh.vertices = vertices;
            planetMeshFilter.mesh.RecalculateNormals();
        }
    }

}
