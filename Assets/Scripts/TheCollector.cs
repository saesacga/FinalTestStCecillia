using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TheCollector : MonoBehaviour
{
    [SerializeField] private Camera _fpsCamera;
    [SerializeField] private int _damage;
    [SerializeField] private float _attractRadius;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Image _objectToEjectSelected;
    [SerializeField] private GameObject _ejectedObject;
    [Range(0f,15f)]
    [SerializeField] private float _attractVelocity;
    
    private ItemData _itemDataSelected;
    private bool _canCollect = false;
    
    public static event Action<bool> OnMoving;
    
    private void OnEnable()
    {
        InventorySlot.OnUseButton += SelectToEject;
    }
    private void OnDisable()
    {
        InventorySlot.OnUseButton -= SelectToEject;
        OnMoving?.Invoke(false);
    }

    private void Update()
    {
        if (ActionMapReference.playerMap.Farming.Destroy.IsPressed())
        {
            DestroyObjects();
        }
        else if (ActionMapReference.playerMap.Farming.Collect.IsPressed())
        {
            AttractObjects();
            _canCollect = ActionMapReference.playerMap.Farming.Collect.IsPressed();
        }
        else if (ActionMapReference.playerMap.Farming.Eject.WasPressedThisFrame())
        {
            if (_itemDataSelected != null)
            {
                EjectItems(_itemDataSelected);
            }
        }
        else if (ActionMapReference.playerMap.Farming.MoveObject.IsPressed() || ActionMapReference.playerMap.Farming.MoveObject.WasReleasedThisFrame())
        {
            MoveObjects();
        }
    }

    private void DestroyObjects()
    {
        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15f))
        {
            if (hit.transform.CompareTag("Destructable"))
            {
                hit.collider.gameObject.GetComponent<IDestructible>().Destruct(_damage);
            }
        }
    }

    private void AttractObjects()
    {
        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.SphereCast(ray, _attractRadius, out hit, 15f))
        {
            if (hit.transform.CompareTag("Loot"))
            {
                GameObject attractable = hit.collider.gameObject;
                attractable.transform.position = Vector3.Lerp(attractable.transform.position,this.transform.position,_attractVelocity * Time.deltaTime);
            }
        }
    }

    private void EjectItems(ItemData itemData)
    {
        if (Inventory._itemDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            _inventory.RemoveItem(itemData);
            GameObject _ejectedInstances = Instantiate(_ejectedObject, transform.position, transform.rotation);
            _ejectedInstances.GetComponent<SpriteRenderer>().sprite = itemData.icon;
            _ejectedInstances.GetComponent<SpriteRenderer>().enabled = true;
            _ejectedInstances.GetComponent<EjectedMaterial>().itemData = itemData;
            
            #region Objetivo a seguir

            Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if(Physics.Raycast(ray, out hit, 20f)) { targetPoint = hit.point; }
            else { targetPoint = ray.GetPoint(75); }

            #endregion

            _ejectedInstances.GetComponent<EjectedMaterial>().targetPoint = targetPoint;
        }
    }
    
    private void MoveObjects()
    {
        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15f))
        {
            if (hit.transform.CompareTag("Moveable"))
            {
                GameObject moveable = hit.collider.gameObject;
                if (ActionMapReference.playerMap.Farming.MoveObject.IsPressed())
                {
                    OnMoving?.Invoke(true);
                }
                else
                {
                    OnMoving?.Invoke(false);
                }
            }
            else
            { 
                OnMoving?.Invoke(false);
            }
        }
        else
        {
            OnMoving?.Invoke(false);
        }
    }
    
    private void SelectToEject(ItemData itemData)
    {
        if (itemData != null)
        {
            _objectToEjectSelected.sprite = itemData.icon;
            _objectToEjectSelected.enabled = true;
            _itemDataSelected = itemData;
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<IAttractable>() != null && _canCollect)
        {
            collider.GetComponent<IAttractable>().CollectOnAttract();
        }
    }
}
