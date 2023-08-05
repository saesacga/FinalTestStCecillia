using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Pathfinding;

public class Destructible : MonoBehaviour, IDestructible
{
    [SerializeField] private GameObject _lootPrefab;
    [SerializeField] private float _objectHealth;
    private float _objectHealthForCode;
    private Vector3 _lootSpawn;

    private void OnEnable()
    {
        _objectHealthForCode = _objectHealth;
    }

    public IEnumerator Destruct(int damage)
    {
        _objectHealthForCode -= damage*Time.deltaTime;
        if(_objectHealthForCode<=0)
        {
            int _lootAmount = Random.Range(3, 10);
            gameObject.SetActive(false);
            for (int i = 0; i <= _lootAmount; i++)
            {
                #region Loot Spawn Declaration
                _lootSpawn = new Vector3((Random.Range(this.gameObject.transform.position.x-5,this.gameObject.transform.position.x+5)),this.gameObject.transform.position.y+3,(Random.Range(this.gameObject.transform.position.z-5,this.gameObject.transform.position.z+5)));
                #endregion
                GameObject.Instantiate(_lootPrefab, _lootSpawn, this.transform.rotation);
            }
            
            yield return new WaitForSeconds(Random.Range(60f,200f));
            gameObject.SetActive(true);
            
        }
    }
}
