using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructible : MonoBehaviour, IConstruible
{
    [SerializeField] private ItemData _materialNeed;
    [SerializeField] private int _amountRequired;
    [SerializeField] private Material _targetMaterial;
    [SerializeField] private GameObject _bubbleMaterial;
    private int _amountProgress = 0;
    public static bool _collectorStep4Completed;
    [HideInInspector] public bool constructed;

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
                _collectorStep4Completed = true;
                GetComponent<MeshRenderer>().material = _targetMaterial;
                GetComponent<Collider>().isTrigger = false;
                _bubbleMaterial.SetActive(false);
                constructed = true;
            }
        } 
    }
}
