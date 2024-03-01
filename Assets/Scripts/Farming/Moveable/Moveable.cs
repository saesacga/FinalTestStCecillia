using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    #region Members

    [SerializeField] private Transform _objectGrabPoint;
    private static Transform _objectGrabPointStatic;
    [SerializeField] private GameObject _moveable;
    private MeshRenderer _movableMeshRenderer;
    [SerializeField] private Material _outlineMaterial;
    private Rigidbody _rigidbody;
    private bool _moving;

    #endregion
    
    private void OnEnable()
    {
        _movableMeshRenderer = GetComponent<MeshRenderer>();
        _movableMeshRenderer.material = new Material(_outlineMaterial);
        TheCollector.OnMoving += Moving;
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnDisable()
    {
        TheCollector.OnMoving -= Moving;
    }

    private void Update()
    {
        if (_objectGrabPoint != null)
        {
            _objectGrabPointStatic = _objectGrabPoint;
        }
    }

    [SerializeField] private Material[] _dissolveMaterials;
    private IEnumerator DestroyWithEffects(GameObject _destroyObject)
    {
        MeshRenderer _currentCubeMesh = _destroyObject.GetComponent<MeshRenderer>();
        _currentCubeMesh.materials = _dissolveMaterials;
        float _dissolveTime;
        while (_currentCubeMesh.material.GetFloat("_Dissolve") < 1)
        {
            _dissolveTime = Mathf.MoveTowards(_currentCubeMesh.material.GetFloat("_Dissolve"), 1f, 1f * Time.deltaTime);
            _currentCubeMesh.material.SetFloat("_Dissolve", _dissolveTime);
            yield return null;
        }
        Destroy(_destroyObject);
    }

    private void FixedUpdate()
    {
        if (_moving)
        {
            Vector3 direction = _objectGrabPointStatic.position - transform.position;
            if (Vector3.Distance(transform.position, _objectGrabPointStatic.position) > 0.5f)
            {
                _rigidbody.AddForce(direction.normalized * 10, ForceMode.VelocityChange);   
            }
        }
    }

    private static GameObject _cubeInstanceObject;
    private static GameObject _currentCube;
    public virtual void Moving(bool moving)
    {
        if (moving)
        {
            if (gameObject == _cubeInstanceObject)
            {
                StartCoroutine(DestroyWithEffects(_currentCube));
                _currentCube = _cubeInstanceObject;
                _cubeInstanceObject = null;
            }

            if (_currentCube == null) { _currentCube = gameObject; }
            if (_cubeInstanceObject == null) { _cubeInstanceObject = Instantiate(_moveable, new Vector3(0f,50f,0f), Quaternion.identity); }
            
            _moving = true;
            _rigidbody.drag = 10; 
            _rigidbody.useGravity = false; 
        }
        else
        {
            if (gameObject == _cubeInstanceObject)
            {
                if (gameObject != _cubeInstanceObject)
                {
                    StartCoroutine(DestroyWithEffects(gameObject));
                }
            }
            _moving = false;
            _rigidbody.drag = 1;
            _rigidbody.useGravity = true;
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && _moving)
        {
            _moving = false;
        }
    }
}