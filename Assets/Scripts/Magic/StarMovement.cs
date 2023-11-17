using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _finalPosition;
    private float _starVelocity = 0f;
    [SerializeField] private AnimationCurve _starVelocityCurve;
    private float _starVelocityOverTime;
    [SerializeField] private LightRay _lightRay;
    [SerializeField] private Sprite _shineStarSprite;
    private bool _activated;
    private bool _finalPositionReached;

    private void Update()
    {
        if (_lightRay._starsCount == _lightRay._starsRequired)
        {
            #region Velocity Curve

            if (_starVelocityOverTime <= 15f)
            {
                _starVelocity = _starVelocityCurve.Evaluate(_starVelocityOverTime);
                _starVelocityOverTime+=0.01f;
            }
            
            #endregion

            transform.position = Vector3.MoveTowards(transform.position, _finalPosition, _starVelocity * Time.deltaTime);

            if (Vector3.Distance(transform.position, _finalPosition) < 1f && _finalPositionReached == false)
            {
                _lightRay._starsInPosition++;
                _finalPositionReached = true;               
            }
        }

        if (_lightRay._starsRequired == _lightRay._starsInPosition) { gameObject.SetActive(false); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") && _activated == false)
        {
            GetComponent<SpriteRenderer>().sprite = _shineStarSprite;
            _lightRay._starsCount++;
            _activated = true;
        }
    }
}
