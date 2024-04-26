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
    [SerializeField] private GameObject _particleChange;
    [SerializeField] private GameObject _particleDestroy;
    private bool _activated;
    private bool _finalPositionReached;

    [SerializeField] private AudioClip[] _starsInPositionSound;

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
                SoundManager.PlaySoundOneShot(_starsInPositionSound);
            }
        }

        if (_lightRay._starsRequired == _lightRay._starsInPosition)
        {
            _particleDestroy = Instantiate(_particleDestroy, transform.position, Quaternion.identity);
            float totalDuration = _particleDestroy.GetComponent<ParticleSystem>().main.duration + _particleDestroy.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
            Destroy(_particleDestroy, totalDuration);
            //Destroy(this.gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") && _activated == false)
        {
            GetComponent<SpriteRenderer>().sprite = _shineStarSprite;
            _particleChange = Instantiate(_particleChange, transform.position, Quaternion.identity);
            float totalDuration = _particleChange.GetComponent<ParticleSystem>().main.duration + _particleChange.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
            Destroy(_particleChange, totalDuration);
            _lightRay._starsCount++;
            _activated = true;
        }
    }
}
