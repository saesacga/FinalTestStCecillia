using System;
using UnityEngine;
using System.Collections;

public class ForestLoop : MonoBehaviour
{
    [SerializeField] private Transform _playerPos;
    [SerializeField] private Transform _forestEnter;
    [SerializeField] private Material _skyboxMaterial;
    [SerializeField] private Color _skyboxOriginalColor;
    [SerializeField] private Color _skyboxFogColor;
    private Color _skyboxLerp;

    void Update() 
    {
        if (Vector3.Distance(_playerPos.position, transform.position) < 20f && RenderSettings.fogEndDistance > 5f)
        {
            RenderSettings.fogEndDistance = Mathf.LerpUnclamped(RenderSettings.fogEndDistance, 8f, 5 * Time.deltaTime);
            _skyboxLerp = Color.LerpUnclamped(_skyboxMaterial.GetColor("_Tint"), _skyboxFogColor, 10 * Time.deltaTime);
            _skyboxMaterial.SetColor("_Tint", _skyboxLerp);
            
        }
        else if (Vector3.Distance(_playerPos.position, transform.position) > 20f)
        {
            RenderSettings.fogEndDistance = Mathf.LerpUnclamped(RenderSettings.fogEndDistance, 500f, 0.1f * Time.deltaTime);
            _skyboxLerp = Color.LerpUnclamped(_skyboxMaterial.GetColor("_Tint"), _skyboxOriginalColor, 2 * Time.deltaTime);
            _skyboxMaterial.SetColor("_Tint", _skyboxLerp);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Palomas")) //Modificar esto equis de
        {
            _playerPos.position = new Vector3(_forestEnter.position.x, _playerPos.position.y, _playerPos.position.z);   
        }
    }
}
