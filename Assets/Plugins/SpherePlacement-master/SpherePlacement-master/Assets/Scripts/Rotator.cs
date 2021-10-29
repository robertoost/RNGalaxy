using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    public float speed = 45;

    private void Update() {
        transform.rotation *= Quaternion.Euler(0, Time.deltaTime * speed, 0);
    }
}
