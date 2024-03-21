using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float _minIntensity;
    [SerializeField] private float _maxIntensity;
    private Light _light;
    private float _flickerLerp;
    void Start()
    {
        _light = GetComponent<Light>();
    }
    
    void Update()
    {
        _light.intensity = Random.Range(_minIntensity, _maxIntensity);
    }
}
