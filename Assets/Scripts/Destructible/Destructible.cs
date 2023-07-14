using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Destructible : MonoBehaviour, IDestructible
{
    [SerializeField] private GameObject _lootPrefab;
    [SerializeField] private float _objectHealth;
    private Vector3 _lootSpawn;
    
    public void Destruct(int damage)
    {
        _objectHealth -= damage*Time.deltaTime;
        if(_objectHealth<=0)
        {
            int _lootAmount = Random.Range(3, 10);
            Destroy(this.gameObject);
            for (int i = 0; i <= _lootAmount; i++)
            {
                #region Loot Spawn Declaration
                _lootSpawn = new Vector3((Random.Range(this.gameObject.transform.position.x-5,this.gameObject.transform.position.x+5)),this.gameObject.transform.position.y+3,(Random.Range(this.gameObject.transform.position.z-5,this.gameObject.transform.position.z+5)));
                #endregion
                GameObject.Instantiate(_lootPrefab, _lootSpawn, this.transform.rotation);
            }
        }
    }
}
