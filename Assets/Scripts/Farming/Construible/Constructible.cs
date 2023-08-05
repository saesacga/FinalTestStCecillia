using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructible : MonoBehaviour, IConstruible
{
    [SerializeField] private ItemData _materialNeed;
    [SerializeField] private int _amountRequired;
    [SerializeField] private Material _targetMaterial;
    private int _amountProgress = 0;
    
    public void Consturct(ItemData itemData)
    {
        if (itemData == _materialNeed)
        {
            if (_amountProgress < _amountRequired)
            {
                _amountProgress++;
            }
            if (_amountProgress == _amountRequired)
            {
                GetComponent<MeshRenderer>().material = _targetMaterial;
                GetComponent<Collider>().isTrigger = false;
            }
        } 
    }
}
