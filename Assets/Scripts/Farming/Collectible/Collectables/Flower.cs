using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flower : ItemsToInventory, ICollectible
{
    
    public ItemData flowerData;
    
    [SerializeField] private SpriteRenderer _interactSprite;
    private bool _collectButtonFlag = false;
    
    private void Update()
    {
        if (ActionMapReference.playerMap.Interaccion.Interactuar.WasPressedThisFrame() && _collectButtonFlag)
        {
            //GetInventoryValues(flowerData);
        }
    }
    
    public void Collect()
    {
        _interactSprite.enabled = true;
        _collectButtonFlag = true;
    }
    public void CannotCollect()
    {
        _interactSprite.enabled = false;
        _collectButtonFlag = false;
    }
}
