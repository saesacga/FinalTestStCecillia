using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestFog : MonoBehaviour
{
    private bool _activateFog;
    [SerializeField] private Material _skyboxMaterial;
    [SerializeField] private Color _skyboxOriginalColor;
    [SerializeField] private Color _skyboxFogColor;
    private Color _skyboxLerp;

    void Update() 
    {
        if (_activateFog && RenderSettings.fogEndDistance > 8f)
        {
            RenderSettings.fogEndDistance = Mathf.LerpUnclamped(RenderSettings.fogEndDistance, 8f, 8 * Time.deltaTime);
            _skyboxLerp = Color.LerpUnclamped(_skyboxMaterial.GetColor("_Tint"), _skyboxFogColor, 10 * Time.deltaTime);
            _skyboxMaterial.SetColor("_Tint", _skyboxLerp);
            
        }
        else if (_activateFog == false && RenderSettings.fogEndDistance < 500f)
        {
            RenderSettings.fogEndDistance = Mathf.LerpUnclamped(RenderSettings.fogEndDistance, 500f, 0.5f * Time.deltaTime);
            _skyboxLerp = Color.LerpUnclamped(_skyboxMaterial.GetColor("_Tint"), _skyboxOriginalColor, 2 * Time.deltaTime);
            _skyboxMaterial.SetColor("_Tint", _skyboxLerp);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player")) { _activateFog = true; }
    }
    private void OnTriggerExit(Collider collider)
    { 
        if (collider.CompareTag("Player")) { _activateFog = false; }
    }
}
