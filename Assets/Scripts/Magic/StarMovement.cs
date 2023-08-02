using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _finalPosition;
    [SerializeField] private float _starVelocity;
    [SerializeField] private AnimationCurve _starVelocityCurve;
    private float _starVelocityOverTime;
    [SerializeField] private Constellations _constellation;
    [SerializeField] private Sprite _shineStarSprite;
    private bool _activated;
    private bool _finalPositionReached;

    private void Update()
    {
        if (_constellation._starsCount == _constellation._starsRequired)
        {
            #region Velocity Curve

            if (_starVelocityOverTime <= 15f)
            {
                _starVelocity = _starVelocityCurve.Evaluate(_starVelocityOverTime);
                _starVelocityOverTime+=0.01f;
            }
            
            #endregion

            transform.position = Vector3.MoveTowards(transform.position, _finalPosition, _starVelocity * Time.deltaTime);
            
            if (transform.position == _finalPosition && _finalPositionReached == false)
            {
                _constellation._starsInPosition++;
                _finalPositionReached = true;
            }
        }

        if (_constellation._starsRequired == _constellation._starsInPosition) { gameObject.SetActive(false); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") && _activated == false)
        {
            GetComponent<SpriteRenderer>().sprite = _shineStarSprite;
            _constellation._starsCount++;
            _activated = true;
        }
    }
}
