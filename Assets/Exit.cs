using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    bool keyDown = false;
    float timer = 0;
    float delay = 0.5f;

    public void Update()
    {
        keyDown = Input.GetKey(KeyCode.Escape);
        
        if (keyDown)
        {
            timer += Time.deltaTime;
            if (timer > delay)
            {
                Application.Quit();
            }
        } else
        {
            timer = 0;
        }
    }
}
