using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    
    [SerializeField] private TextMeshProUGUI _stackSizeDisplay;
    private int _progressText;
    private bool _firstMatThrow = true;

    private void OnEnable()
    {
        _progressText = _amountRequired;
        _dissolveSubs = 0.4f / _amountRequired;
        _targetObjectMeshR = _targetObject.GetComponent<MeshRenderer>();
        _targetObjectMeshR.material = new Material(_dissolveMaterial);
        _stackSizeDisplay.SetText((_amountRequired).ToString());
    }

    public void Consturct(ItemData itemData)
    {
        if (itemData == _materialNeed)
        {
            if (_amountProgress < _amountRequired)
            {
                if (_firstMatThrow) { _dissolveNumber -= 0.1f; _firstMatThrow = false; }
                _dissolveNumber -= _dissolveSubs;
                _amountProgress++;
                _progressText--;
                _stackSizeDisplay.SetText((_progressText).ToString());
            }
            if (_amountProgress == _amountRequired)
            {
                _targetObject.GetComponent<Collider>().enabled = true;
                _bubbleMaterial.SetActive(false);
                constructed = true;
            }
        } 
    }

    private void Update()
    {
        _dissolveLerp = Mathf.Lerp(_targetObjectMeshR.material.GetFloat("_Dissolve"), _dissolveNumber, 1f * Time.deltaTime); 
        _targetObjectMeshR.material.SetFloat("_Dissolve", _dissolveLerp);
        if (constructed && _targetObjectMeshR.material.GetFloat("_Dissolve") > 0)
        {
            _dissolveLerp = Mathf.MoveTowards(_targetObjectMeshR.material.GetFloat("_Dissolve"), 0, 0.5f * Time.deltaTime); 
            _targetObjectMeshR.material.SetFloat("_Dissolve", _dissolveLerp);
        } // Terminar de construir objeto si no ha acabad
    }
}
