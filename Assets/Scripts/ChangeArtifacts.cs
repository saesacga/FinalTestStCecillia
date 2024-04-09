using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeArtifacts : MonoBehaviour
{
    [SerializeField] private Animator _gaunletAnimator;
    [SerializeField] private Animator _collectorAnimator;
    [SerializeField] private Animator _amaAnimator;
    [SerializeField] private bool _activateCollector;
    [SerializeField] private bool _activateAMA;
    [SerializeField] private bool _deactivateAMA;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && _activateCollector)
        {
            GetComponent<Animator>().SetBool("openChest", true);
            _audioSource.Play();
            _gaunletAnimator.SetBool("toggle", false);
            _collectorAnimator.SetBool("toggle", true);
            _activateCollector = false;
        }
        else if (collision.CompareTag("Player") && _activateAMA)
        {
            GetComponent<Animator>().SetBool("openChest", true);
            _audioSource.Play();
            _collectorAnimator.SetBool("toggle", false);
            _amaAnimator.SetBool("toggle", true);
            _activateAMA = false;
        }
        else if (collision.CompareTag("Player") && _deactivateAMA)
        {
            _amaAnimator.SetBool("toggle", false);
            _deactivateAMA = false;
        }
    }
}
