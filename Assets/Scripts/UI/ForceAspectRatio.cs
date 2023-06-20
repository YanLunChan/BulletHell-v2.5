using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ForceAspectRatio : MonoBehaviour
{
    [SerializeField] private float aspectRatio = 16f / 9f;

    void Start()
    {
        ForceScreenSize();
    }

    public void ForceScreenSize() 
    {
        float currentRatio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = currentRatio / aspectRatio;

        if (scaleHeight < 1f)
        {
            Rect rect = Camera.main.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;

            Camera.main.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;

            Rect rect = Camera.main.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            Camera.main.rect = rect;
        }
    }
}
