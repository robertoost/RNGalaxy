using Habrador_Computational_Geometry;
using RNGalaxy;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RNGalaxy.Plate;

[CustomEditor(typeof(CrustGenerator))]
public class CrustGeneratorEditor : Editor
{
    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            CrustGenerator t = target as CrustGenerator;

            // Initialize crustgenerater variables.
            Transform tr = t.transform;
            Quaternion rotation = tr.rotation;
            Vector3 center = tr.position;
            float radius = t.radius;
            List<Plate> plates = t.crust.plates;

            Random.InitState(t.seed);


            for (int i = 0; i < plates.Count; i++)
            {
                Plate plate = plates[i];
                PlateType plateType = plate.plateType;

                Handles.color = plateType == PlateType.continental ? Color.green : Color.blue;

                Vector3 drawPoint = plate.center;
                drawPoint = rotation * (radius * drawPoint.normalized) + center;

                Handles.SphereHandleCap(i + 500, drawPoint, Quaternion.identity, 0.1f, EventType.Repaint);

                foreach (PlateEdge edge in plate.edges)
                {
                    Handles.color = Color.red;
                    Vector3 p1 = radius * edge.start.normalized;
                    Vector3 p2 = radius * edge.end.normalized;
                    p1 = rotation * p1 + center;
                    p2 = rotation * p2 + center;

                    // calculate normal https://docs.unity3d.com/560/Documentation/Manual/ComputingNormalPerpendicularVector.html
                    Vector3 side1 = p1 - center;
                    Vector3 side2 = p2 - center;
                    Vector3 normal = Vector3.Cross(side2, side1);

                    float angle = Vector3.Angle(side1, side2);
                    Handles.DrawWireArc(center, normal, p2, angle, radius);
                }
            }
        }
    }
}
