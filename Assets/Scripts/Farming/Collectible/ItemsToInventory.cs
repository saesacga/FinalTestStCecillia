using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemsToInventory : MonoBehaviour, IAttractable
{
    public static event Action<ItemData> OnItemCollected;
    
    public ItemData itemData;
    
    public void CollectOnAttract()
    {
        Destroy(this.gameObject);
        OnItemCollected?.Invoke(itemData);
    }
}
