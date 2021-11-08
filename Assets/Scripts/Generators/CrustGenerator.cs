using Habrador_Computational_Geometry;
using System.Collections.Generic;
using UnityEngine;
using static RNGalaxy.Plate;
using Noise;

namespace RNGalaxy
{
    public class CrustGenerator : MonoBehaviour
    {
        public int seed = 20;
        public int n = 30;
        public float radius = 8;
        public float amplitude = 1f;
        public Crust crust;

        public HashSet<Vector3> points;
        public List<Plate> plates;

        private MeshFilter meshFilter;
        private Mesh originalMesh;

        private Color landColor;
        private Color waterColor;
        

        // Update is called once per frame
        void OnValidate()
        {

            meshFilter = GetComponent<MeshFilter>();
            if (!originalMesh)
            {
                originalMesh = Instantiate<Mesh>(meshFilter.sharedMesh);
            }

            crust = new Crust(n, radius, originalMesh, randomSeed: seed);

            if (Application.isPlaying)
            {
                Random.InitState(seed);

                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.material.color = Random.ColorHSV(0,1,0.75f, 1f);
                MeshRenderer childMeshRenderer = GetComponentsInChildren<MeshRenderer>()[1];
                childMeshRenderer.material.color = Random.ColorHSV(0, 1, 0.75f, 1f);
                Debug.Log(childMeshRenderer.material.name);
                ChangeVertices(crust);
            }
        }

        private void Awake()
        {
            originalMesh = Instantiate<Mesh>(meshFilter.sharedMesh);
            ChangeVertices(crust);
        }
        
        // Changes vertex position on mesh to represent ocean depth.
        private void ChangeVertices(Crust crust)
        {
            List<Plate> plates = crust.plates;
            Vector3[] vertices = originalMesh.vertices;
            Vector3[] newVertices = new Vector3[vertices.Length];

            foreach(Plate plate in plates)
            {
                foreach(int vertexID in plate.vertices)
                {

                    Vector3 vertex = vertices[vertexID];
                    if(plate.plateType == PlateType.oceanic)
                    {
                        newVertices[vertexID] = vertex - vertex.normalized;
                    } else
                    {
                        // TODO: Apply vertical noise here
                        //newVertices[vertexID] = vertex
                        float height = Perlin.Noise(vertex.normalized * radius);
                        float roughness = amplitude * 0.01f;
                        newVertices[vertexID] = vertex + (vertex.normalized * height * roughness);
                    }
                }
            }
            meshFilter.mesh.vertices = newVertices;
            meshFilter.mesh.RecalculateNormals();
        }
    }
}