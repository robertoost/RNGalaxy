using UnityEngine;
using RNGalaxy;

public class FibionacciSphereDemo : MonoBehaviour
{
    public int n = 8;
    public float radius = 8;
    private Vector3[] points;

    private void OnValidate()
    {
        points = FibionacciSphere.GeneratePoints(n, radius);
    }

    private void OnDrawGizmos()
    {
        for(int i=0; i < points.Length; i++)
        {
            // Set Gizmos hue color.
            float hue = (i + 1f) / points.Length;
            Color color = Color.HSVToRGB(hue, 1, 1);
            Gizmos.color = color;

            // Draw sphere at the position of this point relative to the transform position.
            Vector3 point = points[i];
            Vector3 drawPos = point + transform.position;
            drawPos = transform.rotation * drawPos;
            Gizmos.DrawSphere(drawPos, 0.1f);
        }
    }
}
