using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    #region Members

    [SerializeField] private Transform _objectGrabPoint;
    [SerializeField] private float _pickUpForce = 150;
    private Rigidbody _rigidbody;
    private bool moving;

    #endregion
    
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
            Vector3 direction = _objectGrabPoint.position - transform.position;
            if (Vector3.Distance(transform.position, _objectGrabPoint.position) > 0.5f)
            {
                _rigidbody.AddForce(direction.normalized * 10, ForceMode.VelocityChange);   
            }
        }
    }

    public virtual void Moving(bool moving)
    {
        if (moving)
        {
            this.moving = true;
            _rigidbody.drag = 10; 
            GetComponent<GravityBody>().useCustomGravity = false; 
        }
        else
        { 
            this.moving = false;
            _rigidbody.drag = 1;
            GetComponent<GravityBody>().useCustomGravity = true;
        }
    }
}
