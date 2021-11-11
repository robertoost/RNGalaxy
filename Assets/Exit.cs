using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    bool keyDown = false;

    public void Update()
    {
        keyDown = Input.GetKey(KeyCode.Escape);

        if (keyDown)
        {
            Application.Quit();
        }
    }
}
