using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemsToInventory : MonoBehaviour, IAttractable
{
    public static event Action<ItemData> OnItemCollected;
    
    public ItemData itemData;

    #region Dissolve

    [SerializeField] private Material _dissolveMaterial;
    [SerializeField] private GameObject _dissolveObject;
    private SpriteRenderer _dissolveObjectSpriteRenderer;
    private float _dissolveLerp = 0.95f;
    private bool _spawnEffect;

    #endregion

    private void OnEnable()
    {
        _spawnEffect = true;
        _dissolveObjectSpriteRenderer = _dissolveObject.GetComponent<SpriteRenderer>();
        _dissolveObjectSpriteRenderer.material = new Material(_dissolveMaterial);
        _dissolveObjectSpriteRenderer.material.SetFloat("_Dissolve", 0.95f);
    }

    public void CollectOnAttract()
    {
        OnItemCollected?.Invoke(itemData);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (_spawnEffect)
        {
            if (_dissolveLerp <= 0.25) { _spawnEffect = false; return; }
            _dissolveLerp = Mathf.MoveTowards(_dissolveObjectSpriteRenderer.material.GetFloat("_Dissolve"), 0.25f, 1f * Time.deltaTime); 
            _dissolveObjectSpriteRenderer.material.SetFloat("_Dissolve", _dissolveLerp);
        }
    }
}
