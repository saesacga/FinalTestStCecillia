using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estatua : MonoBehaviour
{
    private void Update()
    {
        if (GetComponentInParent<Constructible>().constructed)
        {
            GetComponent<SpriteRenderer>().color = new Color (255, 255, 255, 1);
        }
        
    }
}
