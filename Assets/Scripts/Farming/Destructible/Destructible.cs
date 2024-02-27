using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using SpriteGlow;
using Pathfinding;

public class Destructible : MonoBehaviour, IDestructible
{
    [SerializeField] private GameObject _lootPrefab;
    [SerializeField] private float _objectHealth;
    private float _objectHealthForCode;
    private Vector3 _lootSpawn;
    public bool destroy;

    #region Dissolve
    
    [SerializeField] private Material _dissolveMaterial;
    [SerializeField] private GameObject _dissolveObject;
    private SpriteRenderer _dissolveObjectSpriteRenderer;
    private float _dissolveAdd;
    private float _dissolveNumber;
    private float _dissolveLerp = 0.25f;

    private SpriteGlowEffect _spriteGlow;
    private bool _respawn;

    #endregion
    

    private void OnEnable()
    {
        destroy = false;
        _respawn = true;
        
        _dissolveObjectSpriteRenderer = _dissolveObject.GetComponent<SpriteRenderer>();
        _dissolveObjectSpriteRenderer.material = new Material(_dissolveMaterial);
        _dissolveObjectSpriteRenderer.material.SetFloat("_Dissolve", 0.95f);
        
        _objectHealthForCode = _objectHealth;
        _dissolveNumber = 0.25f;
        _dissolveAdd = 10f / _objectHealth;
        _spriteGlow = GetComponent<SpriteGlowEffect>();
    }
    
    public IEnumerator Destruct(int damage)
    {
        _objectHealthForCode -= damage*Time.deltaTime;
        _dissolveNumber += _dissolveAdd*Time.deltaTime;
        
        if(_objectHealthForCode<=0)
        {
            destroy = true;
            GetComponent<Collider>().enabled = false;
            int _lootAmount = Random.Range(4, 10);
            for (int i = 0; i <= _lootAmount; i++)
            {
                #region Loot Spawn Declaration
                _lootSpawn = new Vector3((Random.Range(this.gameObject.transform.position.x-5,this.gameObject.transform.position.x+5)),this.gameObject.transform.position.y+3,(Random.Range(this.gameObject.transform.position.z-5,this.gameObject.transform.position.z+5)));
                #endregion
                GameObject.Instantiate(_lootPrefab, _lootSpawn, this.transform.rotation);
            }
            
            while (_spriteGlow.glowColor.a > 0)
            {
                _spriteGlow.glowColor.a = Mathf.MoveTowards(_spriteGlow.glowColor.a, 0f, 1f * Time.deltaTime);
                yield return null;
            }  
            
            gameObject.SetActive(false);
            yield return new WaitForSeconds(Random.Range(30f,150f));
            gameObject.SetActive(true);
        }
    }
    
    [Range(1.0f, 10.0f)]
    [SerializeField] private float _lerpTime = 5f;
    private void Update()
    {
        if (_respawn)
        {
            if (_dissolveLerp <= 0.25f) { _respawn = false; GetComponent<Collider>().enabled = true; return; }
            _dissolveLerp = Mathf.MoveTowards(_dissolveObjectSpriteRenderer.material.GetFloat("_Dissolve"), 0.25f, 1f * Time.deltaTime); 
            _dissolveObjectSpriteRenderer.material.SetFloat("_Dissolve", _dissolveLerp);
        }
        else
        {
            _dissolveLerp = Mathf.LerpUnclamped(_dissolveObjectSpriteRenderer.material.GetFloat("_Dissolve"), _dissolveNumber, _lerpTime * Time.deltaTime); 
            _dissolveObjectSpriteRenderer.material.SetFloat("_Dissolve", _dissolveLerp);
        }
    }
}
