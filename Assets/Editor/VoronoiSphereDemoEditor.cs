using UnityEngine;
using UnityEditor;
using Habrador_Computational_Geometry;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(VoronoiSphereDemo))]
public class VoronoiSphereDemoEditor : Editor
{
    // Custom in-scene UI for when ExampleScript
    // component is selected.
    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            var t = target as VoronoiSphereDemo;
            var tr = t.transform;
            var pos = tr.position;
            // display an orange disc where the object is
            var color = new Color(1, 0.8f, 0.4f, 1);
            Handles.color = color;
            Handles.DrawWireDisc(pos, tr.up, 1.0f);
            // display object "value" in scene
            GUI.color = color;

            Random.InitState(t.seed);

            for (int i = 0; i < t.voronoiCellsList.Count; i++)
            {
                VoronoiCell3 cell = t.voronoiCellsList[i];
                Quaternion rotation = t.transform.rotation;
                Vector3 position = t.transform.position;
                float radius = t.radius;
                
                Color col = Random.ColorHSV(0, 1);
                Handles.color = col;
                Vector3 drawPoint = cell.sitePos.ToVector3();
                drawPoint = rotation * drawPoint + position;

                Handles.SphereHandleCap(i + 500, drawPoint, Quaternion.identity, 0.1f, EventType.Repaint);

                foreach (VoronoiEdge3 edge in cell.edges)
                {
                    Handles.color = col;
                    Vector3 p1 = radius * edge.p1.ToVector3().normalized;
                    Vector3 p2 = radius * edge.p2.ToVector3().normalized;
                    p1 = rotation * p1 + position;
                    p2 = rotation * p2 + position;

                    // calculate normal https://docs.unity3d.com/560/Documentation/Manual/ComputingNormalPerpendicularVector.html
                    Vector3 side1 = p1 - position;
                    Vector3 side2 = p2 - position;
                    Vector3 normal = Vector3.Cross(side2, side1);


                    float angle = Vector3.Angle(side1, side2);
                    Handles.DrawWireArc(position, normal, p2, angle, radius);
                }
            }

        }

    }
}