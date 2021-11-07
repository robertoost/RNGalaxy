using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGPU : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SystemInfo.supportsComputeShaders);
    }
}
