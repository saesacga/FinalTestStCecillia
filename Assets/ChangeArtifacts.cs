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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && _activateCollector)
        {
            _gaunletAnimator.SetBool("toggle", false);
            _collectorAnimator.SetBool("toggle", true);
        }
        else if (collision.CompareTag("Player") && _activateAMA)
        {
            _collectorAnimator.SetBool("toggle", false);
            _amaAnimator.SetBool("toggle", true);
        }
        else if (collision.CompareTag("Player") && _deactivateAMA)
        {
            _amaAnimator.SetBool("toggle", false);
        }
    }
}
