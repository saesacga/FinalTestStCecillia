using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    [SerializeField] private Transform _objectGrabPoint;
    [SerializeField] private float _pickUpForce = 150;
    private Rigidbody _rigidbody;
    private bool moving;
    private void OnEnable()
    {
        TheCollector.OnMoving += Moving;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnDisable()
    {
        TheCollector.OnMoving -= Moving;
    }
    
    private void FixedUpdate()
    {
        if (moving)
        {
            _rigidbody.MovePosition(_objectGrabPoint.position);            
        }

    }

    private void MoveObject()
    {
        if (Vector3.Distance(transform.position, _objectGrabPoint.position) > 0.1f)
        {
            Vector3 moveDirection = (_objectGrabPoint.position - transform.position);
            _rigidbody.AddForce(moveDirection * _pickUpForce);
        }   
    }

    public virtual void Moving(bool moving)
    {
        if (moving)
        {
            this.moving = true;
            //transform.parent = _objectGrabPoint; 
            //_rigidbody.useGravity = false;
            _rigidbody.drag = 10; 
            GetComponent<GravityBody>().useCustomGravity = false; 
            MoveObject(); //Cute animation
        }
        else
        { 
            this.moving = false;
            //transform.parent = null; 
            //GetComponent<Rigidbody>().useGravity = true;
            _rigidbody.drag = 1;
            GetComponent<GravityBody>().useCustomGravity = true;
        }
    }
}
