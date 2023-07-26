using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    [SerializeField] private GravityAttractor _gravityAttractor;
    [SerializeField] private float _gravityMultiplier = 1f;
    private Transform _transform;

    private void Start()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().useGravity = false;
        this._transform = transform;
    }

    private void FixedUpdate()
    {
        _gravityAttractor.GravityAttraction(this._transform, this._gravityMultiplier);
    }
}
