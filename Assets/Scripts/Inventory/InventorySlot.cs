using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI stackSize;

    public InventoryItem inventoryItemSlotRef;

    public static event Action<ItemData> OnUseButton;
    
    public void ClearSlot()
    {
        icon.enabled = false;
        stackSize.enabled = false;
    }

    public void DrawSlot(InventoryItem item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }
        
        icon.enabled = true;
        stackSize.enabled = true;

        icon.sprite = item.itemData.icon;
        stackSize.text = item.stackSize.ToString();

        inventoryItemSlotRef = item;
    }

    public void UseItem()
    {
        OnUseButton?.Invoke(inventoryItemSlotRef.itemData);
    }
}


