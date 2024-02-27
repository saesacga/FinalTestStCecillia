using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructible : MonoBehaviour, IConstruible
{
    [SerializeField] private ItemData _materialNeed;
    [SerializeField] private int _amountRequired;
    [SerializeField] private GameObject _bubbleMaterial;
    [SerializeField] private GameObject _targetObject;
    private MeshRenderer _targetObjectMeshR;
    private int _amountProgress;
    
    [HideInInspector] public bool constructed;

    [SerializeField] private Material _dissolveMaterial;
    private float _dissolveSubs;
    private float _dissolveNumber = 0.75f;
    private float _dissolveLerp = 0.75f;

    private void OnEnable()
    {
        _dissolveSubs = 0.7f / _amountRequired;
        _targetObjectMeshR = _targetObject.GetComponent<MeshRenderer>();
        _targetObjectMeshR.material = new Material(_dissolveMaterial);
    }

    public void Consturct(ItemData itemData)
    {
        if (itemData == _materialNeed)
        {
            if (_amountProgress < _amountRequired)
            {
                _dissolveNumber -= _dissolveSubs;
                _amountProgress++;
            }
            if (_amountProgress == _amountRequired)
            {
                _targetObject.GetComponent<Collider>().isTrigger = false;
                _bubbleMaterial.SetActive(false);
                constructed = true;
            }
        } 
    }

    private void Update()
    {
        _dissolveLerp = Mathf.Lerp(_targetObjectMeshR.material.GetFloat("_Dissolve"), _dissolveNumber, 0.5f * Time.deltaTime); 
        _targetObjectMeshR.material.SetFloat("_Dissolve", _dissolveLerp);
    }
}
