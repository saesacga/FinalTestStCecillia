using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
//ESTE SCRIPT MANEJA LOS VALORES INTERNOS DEL INVENTARIO, ALMACENA, ELIMINA, GESTIONA LISTAS.
//LO QUE SE DESPLIEGA VISUALMENTE DENTRO DEL JUEGO VA DE LA MANO DE OTROS SCRIPTS
{
    #region DECLARATIONS
    public static event Action<List<InventoryItem>> OnInventoryChange;
    
    [HideInInspector] public static List<InventoryItem> inventoryItems = new List<InventoryItem>();
    
    //El diccionario se usará para los objetos stackeables:
    public static Dictionary<ItemData, InventoryItem> _itemDictionary = new Dictionary<ItemData, InventoryItem>();

    [SerializeField]private InventoryManager _inventoryManager;
    
    [SerializeField] private GameObject _firstSelectedReference;
    private static GameObject _firstSelectedInv;
    private static Animator _animator;
    private static bool inventoryOpen = true;

    [HideInInspector] public static bool reachSlotLimit;
    #endregion
    
    private void OnEnable()
    {
        _firstSelectedInv = _firstSelectedReference;
        reachSlotLimit = false;
        _animator = GetComponent<Animator>();
        ItemsToInventory.OnItemCollected += AddItem;
    }
    private void OnDisable()
    {
        ItemsToInventory.OnItemCollected -= AddItem;
    }

    private void Update()
    {
        if (ActionMapReference.playerMap.PauseMap.Inventory.WasPressedThisFrame())
        {
            OpenInventory();
        }
    }
    
    public void AddItem(ItemData itemData)
    {
        if (_inventoryManager.inventorySlots.Count > inventoryItems.Count)//hay slots disponibles
        {
            //Revisamos si la clave (ItemData) ya ha sido almacenada previamente:
            if (_itemDictionary.TryGetValue(itemData, out InventoryItem item) && itemData.itsStackeable)
            {
                if (item.stackSize < itemData.stackLimit)
                {
                    item.AddToStack();
                    OnInventoryChange?.Invoke(inventoryItems);
                }
            }
            else //En caso de que no exista o no sea stackeable
            {
                InventoryItem newItem = new InventoryItem(itemData);
                inventoryItems.Add(newItem);
                OnInventoryChange?.Invoke(inventoryItems);
                if (itemData.itsStackeable) { _itemDictionary.Add(itemData, newItem); }
            }
        }
        if(_inventoryManager.inventorySlots.Count <= inventoryItems.Count)
        {
            reachSlotLimit = true;
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        //Revisamos si la clave (ItemData) ya ha sido almacenada previamente:
        if (_itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveFromStack();
            if (item.stackSize == 0)
            {
                inventoryItems.Remove(item);
                _itemDictionary.Remove(itemData);
            }
            OnInventoryChange?.Invoke(inventoryItems);
        }
        else
        {
            Debug.Log("No quedan objetos de este tipo");
        }
    }

    public static void OpenInventory()
    {
        _animator.SetBool("ToggleInventory", inventoryOpen);
        if (inventoryOpen)
        {
            inventoryOpen = !inventoryOpen;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedInv);
            ActionMapReference.playerMap.PauseMap.Pause.Disable();
            ActionMapReference.ActivateUINavigation();
        }
        else if (inventoryOpen == false)
        {
            inventoryOpen = !inventoryOpen;
            EventSystem.current.SetSelectedGameObject(null);
            ActionMapReference.playerMap.PauseMap.Pause.Enable();
            ActionMapReference.ActivateAllMaps();
        }
    }
}
