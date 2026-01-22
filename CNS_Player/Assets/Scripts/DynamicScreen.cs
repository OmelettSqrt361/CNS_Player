using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicScreen : MonoBehaviour
{
    int lastWidth;
    int lastHeight;

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        ApplyResize();
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            ApplyResize();
        }
    }

    void ApplyResize()
    {
        Screen.SetResolution(Screen.width, Screen.height, true);
    }
}
