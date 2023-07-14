using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    private void OnEnable()
    {
        TheCollector.OnMoving += Moving;
    }
    private void OnDisable()
    {
        TheCollector.OnMoving -= Moving;
    }
    
    [SerializeField] private GameObject _moveWith;
    //protected static GameObject moveWithStatic;
    //private void Start() { moveWithStatic = _moveWith; }

    protected virtual void Moving(bool moving)
    {
        if (moving)
        {
            transform.SetParent(_moveWith.transform);
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            transform.SetParent(null);
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().drag = 1;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
