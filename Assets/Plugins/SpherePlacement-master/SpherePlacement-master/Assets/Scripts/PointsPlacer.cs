using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsPlacer : MonoBehaviour {
    public enum DistributionType {
        Linear,
        Random,
        FibonacciDisc,
        FibonacciSphere
    }

    public DistributionType distribution;
    public int n = 8;
    public float radius = 8;
    public Transform prefab;

    private System.Func<int, Vector3>[] distFuncs => new System.Func<int, Vector3>[] {
        Linear,
        Rand,
        FibDisc,
        FibSphere,
    };

    private Vector3 Linear(int i) {
        return Vector3.right * i * radius;
    }

    private Vector3 Rand(int i) {
        return Random.insideUnitSphere.normalized * radius;
    }


    private Vector3 FibDisc(int i) {
        var k = i + .5f;
        var r = Mathf.Sqrt((k) / n);
        var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

        var x = r * Mathf.Cos(theta) * radius;
        var y = r * Mathf.Sin(theta) * radius;

        return new Vector3(x, y, 0);
    }

    private Vector3 FibSphere(int i) {
        var k = i + .5f;

        var phi = Mathf.Acos(1f - 2f * k / n);
        var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;

        var x = Mathf.Cos(theta) * Mathf.Sin(phi);
        var y = Mathf.Sin(theta) * Mathf.Sin(phi);
        var z = Mathf.Cos(phi);

        return new Vector3(x, y, z) * radius;
    }

    private void OnValidate() {
        Place();
    }

    [ContextMenu("Generate")]
    private void Generate() {
        GenerateDist(distFuncs[(int)distribution]);
    }

    [ContextMenu("Place")]
    private void Place() {
        Place(distFuncs[(int)distribution]);
    }

    private void GenerateDist(System.Func<int, Vector3> pfunc) {
        Clear();

        for (int i = 0; i < n; i++) {
            var inst = Instantiate(prefab);
            inst.SetParent(transform);
            inst.localPosition = pfunc(i);
            inst.gameObject.SetActive(true);
        }
    }

    private void Place(System.Func<int, Vector3> pfunc) {
        for (int i = 0; i < n; i++) {
            transform.GetChild(i).localPosition = pfunc(i);
        }
    }

    [ContextMenu("Clear")]
    private void Clear() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
