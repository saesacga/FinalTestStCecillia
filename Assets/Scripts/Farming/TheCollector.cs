using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using SpriteGlow;

public class TheCollector : MonoBehaviour
{
    [SerializeField] private Camera _fpsCamera;
    [SerializeField] private int _damage;
    [SerializeField] private float _attractRadius;
    [SerializeField] private Image _objectToEjectSelected;
    [SerializeField] private GameObject _ejectedObject;
    [Range(0f,15f)]
    [SerializeField] private float _attractVelocity;
    [SerializeField] private LayerMask _rayCastLayerMaskIgnore;
    [SerializeField] private GameObject _canvas;
    
    private ItemData _currentItemData;
    private int _stackSize;
    [SerializeField] private TextMeshProUGUI _stackSizeDisplay;
    private bool _canCollect;
    public static event Action<bool> OnMoving;

    #region Outlines

    private MeshRenderer _moveableMat;
    private SpriteGlowEffect _outlineDest;
    private SpriteGlowEffect _outlineLoot;
    private float _lerpTimeMove = 1f;
    private float _lerpTimeDestroy;
    private float _lerpTimeLoot;

    #endregion
    
    private void OnEnable()
    {
        ItemsToInventory.OnItemCollected += InfoToEject;
        _canvas.SetActive(true);
    }
    private void OnDisable()
    {
        ItemsToInventory.OnItemCollected -= InfoToEject;
        OnMoving?.Invoke(false);
        _canvas.SetActive(false);
    }

    private void Start()
    {
        _currentItemData = null;
    }

    private void Update()
    {
        _stackSizeDisplay.SetText(_stackSize.ToString());

        #region Destruir

        if (ActionMapReference.playerMap.Farming.Destroy.IsPressed())
        {
            DestroyObjects();
        }
        else if (ActionMapReference.playerMap.Farming.Destroy.WasReleasedThisFrame())
        {
            GetComponentInChildren<Animator>().Play("CollectorIdleanim");
        }

        #endregion

        #region Recolectar

        else if (ActionMapReference.playerMap.Farming.Collect.IsPressed() || ActionMapReference.playerMap.Farming.Collect.WasReleasedThisFrame())
        {
            if (ActionMapReference.playerMap.Farming.Collect.IsPressed()) { GetComponentInChildren<Animator>().Play("CollectorSuck"); }
            else { GetComponentInChildren<Animator>().Play("CollectorIdleanim"); }
            AttractObjects();
            _canCollect = ActionMapReference.playerMap.Farming.Collect.IsPressed();
        }

        #endregion

        #region Eyectar

        else if (ActionMapReference.playerMap.Farming.Eject.WasPressedThisFrame())
        {
            if (_currentItemData != null)
            {
                EjectItems(_currentItemData);
            }
        }

        #endregion
        
        #region Mover

        else if (ActionMapReference.playerMap.Farming.MoveObject.WasPerformedThisFrame())
        {
            MoveObjects();
        }
        else if (ActionMapReference.playerMap.Farming.MoveObject.WasReleasedThisFrame())
        {
            GetComponentInChildren<Animator>().Play("CollectorIdleanim");
            OnMoving?.Invoke(false);
        }

        #endregion
        
        #region Outlines

        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        //Outlines para movibles y destruibles
        if (Physics.Raycast(ray, out hit, 15f))
        {
            if (hit.transform.CompareTag("Moveable"))
            {
                if (_moveableMat != hit.collider.gameObject.GetComponent<MeshRenderer>())
                {
                    if (_moveableMat != null) { _moveableMat.material.SetFloat("_Scale", 1f); }
                    _moveableMat = hit.collider.gameObject.GetComponent<MeshRenderer>();
                }
                else
                {
                    if (_lerpTimeMove <= 1.03f) { _lerpTimeMove += 0.003f; }
                    _moveableMat.material.SetFloat("_Scale",  _lerpTimeMove);
                }
            }
            else
            {
                if (_moveableMat != null)
                {
                    if (_lerpTimeMove >= 0.99f) { _lerpTimeMove -= 0.001f; }
                    _moveableMat.material.SetFloat("_Scale", _lerpTimeMove);
                }
            }
            
            if (hit.transform.CompareTag("Destructable"))
            {
                if (hit.collider.gameObject.GetComponent<Destructible>().destroy == false)
                {
                    if (_outlineDest != hit.collider.gameObject.GetComponent<SpriteGlowEffect>())
                    {
                        if (_outlineDest != null) { _outlineDest.glowColor.a = 0f; }
                        _outlineDest = hit.collider.gameObject.GetComponent<SpriteGlowEffect>();
                    }
                    else
                    {
                        if (_lerpTimeDestroy <= 1f) { _lerpTimeDestroy += 0.1f; }
                        _outlineDest.glowColor.a = _lerpTimeDestroy;
                    }
                }
                else { return; }
            }
            else
            {
                if (_outlineDest != null)
                { 
                    if (_lerpTimeDestroy >= 0f) { _lerpTimeDestroy -= 0.05f; }
                    _outlineDest.glowColor.a = _lerpTimeDestroy;
                }
            }
        }
        else 
        {
            if (_moveableMat != null)
            {
                if (_lerpTimeMove >= 0.99f) { _lerpTimeMove -= 0.001f; }
                _moveableMat.material.SetFloat("_Scale", _lerpTimeMove);
            }
            if (_outlineDest != null) 
            { 
                if (_lerpTimeDestroy >= 0f) { _lerpTimeDestroy -= 0.05f; }
                _outlineDest.glowColor.a = _lerpTimeDestroy; 
            }
            
        }
        
        //Outlines para loot
        if (Physics.SphereCast(ray, _attractRadius, out hit, 15f, _rayCastLayerMaskIgnore))
        {
            if (hit.transform.CompareTag("Loot"))
            {
                if (_outlineLoot != hit.collider.gameObject.GetComponent<SpriteGlowEffect>())
                {
                    if (_outlineLoot != null) { _outlineLoot.glowColor.a = 0f; }
                    _outlineLoot = hit.collider.gameObject.GetComponent<SpriteGlowEffect>();
                }
                else { if (_lerpTimeLoot <= 1f) { _lerpTimeLoot += 0.1f; } _outlineLoot.glowColor.a = _lerpTimeLoot; }
            }
            else
            {
                if (_outlineLoot != null) { if (_lerpTimeLoot >= 0f) { _lerpTimeLoot -= 0.05f; } _outlineLoot.glowColor.a = _lerpTimeLoot;  }
            }
        }
        else 
        {
            if (_outlineLoot != null) 
            {
                if (_lerpTimeLoot >= 0f) { _lerpTimeLoot -= 0.05f; } 
                _outlineLoot.glowColor.a = _lerpTimeLoot;  
            }
        }

        #endregion
    }

    private void DestroyObjects()
    {
        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15f))
        {
            if (hit.transform.CompareTag("Destructable"))
            {
                if (hit.collider.gameObject.GetComponent<Destructible>().destroy == false)
                {
                    GetComponentInChildren<Animator>().Play("DestroyCollector");
                    StartCoroutine(hit.collider.gameObject.GetComponent<IDestructible>().Destruct(_damage));
                }
                else
                {
                    GetComponentInChildren<Animator>().Play("CollectorIdleanim");
                }
            }
            else
            {
                GetComponentInChildren<Animator>().Play("CollectorIdleanim");
            }
        }
        else
        {
            GetComponentInChildren<Animator>().Play("CollectorIdleanim");
        }
    }
    
    private void AttractObjects()
    {
        Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.SphereCast(ray, _attractRadius, out hit, 15f,_rayCastLayerMaskIgnore))
        {
            if (hit.transform.CompareTag("Loot"))
            {
                GameObject attractable = hit.collider.gameObject;
                attractable.GetComponent<Rigidbody>().useGravity = false;
                attractable.transform.position = Vector3.MoveTowards(attractable.transform.position,this.transform.position,_attractVelocity * Time.deltaTime);
                
                if (ActionMapReference.playerMap.Farming.Collect.WasReleasedThisFrame())
                {
                    attractable.GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
    
    private void EjectItems(ItemData itemData)
    {
        if (_stackSize > 0)
        {
            GameObject _ejectedInstances = Instantiate(_ejectedObject, transform.position, transform.rotation); 
            _ejectedInstances.GetComponent<SpriteRenderer>().sprite = itemData.icon; 
            _ejectedInstances.GetComponent<SpriteRenderer>().enabled = true; 
            _ejectedInstances.GetComponent<EjectedMaterial>().itemData = itemData; 
            GetComponentInChildren<Animator>().Play("ThrowCollector");
            
            #region Objetivo a seguir
        
            Ray ray = _fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
            RaycastHit hit; 
            Vector3 targetPoint; 
            if(Physics.Raycast(ray, out hit, 20f)) { targetPoint = hit.point; }
            else { targetPoint = ray.GetPoint(75); }
        
            #endregion
        
            _ejectedInstances.GetComponent<EjectedMaterial>().targetPoint = targetPoint;
            
            _stackSize--;
            if (_stackSize <= 0)
            {
                _currentItemData = null;
                _objectToEjectSelected.sprite = null;
                _objectToEjectSelected.enabled = false;
            }
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
                GetComponentInChildren<Animator>().Play("MoveCollector");
                hit.collider.GetComponent<Moveable>().Moving(true);
            }
            else
            {
                GetComponentInChildren<Animator>().Play("CollectorIdleanim");
            }
        }
        else
        {
            GetComponentInChildren<Animator>().Play("CollectorIdleanim");
        }
    }
    
    private void InfoToEject(ItemData itemData)
    {
        if (_currentItemData != itemData || _currentItemData == null)
        {
            _objectToEjectSelected.sprite = itemData.icon;
            _objectToEjectSelected.enabled = true;
        }
        
        if (_currentItemData == itemData || _currentItemData == null)
        {
            _stackSize++;
        }
        else if (_currentItemData != itemData)
        {
            _stackSize = 1;
        }
        _currentItemData = itemData;
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<IAttractable>() != null && _canCollect)
        {
            collider.GetComponent<IAttractable>().CollectOnAttract();
        }
    }
}