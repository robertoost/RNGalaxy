using Habrador_Computational_Geometry;
using RNGalaxy;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ContinentFloodFillDemo))]
public class ContinentFloodFillDemoEditor : Editor
{
    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            ContinentFloodFillDemo t = target as ContinentFloodFillDemo;
     
            
            Planet planet = t.planet;
            Random.InitState(planet.randomSeed);
            List<TectonicPlate> plates = planet.plates;

            Vector3 pos = t.transform.position;
            Quaternion rot = t.transform.rotation;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.NotEqual;

            foreach (TectonicPlate plate in plates)
            {
                Handles.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                foreach (VoronoiTile tile in plate.plateTiles)
                {
                    List<Vector3> points = new List<Vector3>();
                    foreach (VoronoiTileEdge tileEdge in tile.edges)
                    {
                        Vector3 point = pos + rot * tileEdge.p1.ToVector3();
                        points.Add(point);
                    }
                    Handles.DrawAAConvexPolygon(points.ToArray());
                }
            }
        }
    }
}
