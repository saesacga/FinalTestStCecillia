using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorRotation : MonoBehaviour
{
    private void Update()
    {
        Vector3 angle = transform.eulerAngles;
        float x = angle.x;
        float y = angle.y;
        float z = angle.z;
        
        if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }

        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }

        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }

        Debug.Log(Mathf.Round(x) + " , " + Mathf.Round(y) + " , " + Mathf.Round(z));
    }
}
