using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDirectionalController : MonoBehaviour
{
    [Range(0f, 180f)] [SerializeField] private float _backAngle = 65f;
    [SerializeField] private Transform _mainTransform;
    private Animator _animator;
    private float animAngle;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        Vector3 camForwardVector = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

        float signedAngle = Vector3.SignedAngle(_mainTransform.forward, camForwardVector, Vector3.up);

        float angle = Mathf.Abs(signedAngle);

        if (angle < _backAngle)
        {
            this.animAngle = 90f;
        }
        else
        {
            this.animAngle = -90f;
        }
        
        _animator.SetFloat("angle", this.animAngle);
    }
}
