using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleColors : MonoBehaviour
{
    [SerializeField] private Material mt;
    
    [ColorUsage(true, true)]
    [SerializeField] private Color[] colors;
    
    void Start()
    {
        StartCoroutine(Cycle());
    }
    
    public IEnumerator Cycle()
    {
        int startColor = 0;
        int endColor = 1;
        while(true)
        {
            for(float interpolant = 0f; interpolant < 1f; interpolant += 0.001f)
            {
                mt.color = Color.Lerp(colors[startColor], colors[endColor], interpolant);
                yield return null;
            }
            startColor = endColor;
            if (endColor == (colors.Length - 1))
            {
                endColor = 0;
            }
            else
            {
                endColor++;
            }
            
        }
    }
}
