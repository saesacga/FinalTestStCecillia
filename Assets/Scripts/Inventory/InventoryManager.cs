using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private void OnEnable()
    {
        Inventory.OnInventoryChange += DrawInventory;
    }
    private void OnDisable()
    {
        Inventory.OnInventoryChange -= DrawInventory;
    }

    void ResetInventory() //Use at your own risk
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);            
        }
        inventorySlots = new List<InventorySlot>();
    }

    void DrawInventory(List<InventoryItem> inventoryItems)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventorySlots[i].DrawSlot(inventoryItems[i]);          
        }
    }

    public void CreateInventorySlot()
    {
        Inventory.reachSlotLimit = false;
        if (inventorySlots.Count < 10)
        {
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.transform.SetParent(this.transform, false);

            InventorySlot newSlotComponent = newSlot.GetComponent<InventorySlot>();
            newSlotComponent.ClearSlot();
        
            inventorySlots.Add(newSlotComponent);
        }
        else
        {
            Debug.Log("No hay más slots disponibles.");
        }
    }
}
