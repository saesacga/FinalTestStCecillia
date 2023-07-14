using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemsToInventory : MonoBehaviour
{
    public static event Action<ItemData> OnItemCollected;
    public void GetInventoryValues(ItemData itemData)
    {
        if (Inventory._itemDictionary.TryGetValue(itemData, out InventoryItem item) && itemData.itsStackeable)
        {
            if (item.stackSize < itemData.stackLimit && Inventory.reachSlotLimit == false)
            {
                Destroy(gameObject); 
                OnItemCollected?.Invoke(itemData);
            }
            else
            {
                Debug.Log("vasllenobb");
            }
        }
        else if(Inventory.reachSlotLimit == false) //En caso de que no exista o no sea stackeable
        {
            Destroy(gameObject); 
            OnItemCollected?.Invoke(itemData);
        }
    }
}
