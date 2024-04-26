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
    [SerializeField] private AudioSource _stringsMusic;
    [SerializeField] private AudioSource _drumsMusic;
    [SerializeField] private AudioSource _bassMusic;
    [SerializeField] private AudioSource _melodyMusic;
    [SerializeField] private PlayerMovement _playerMovement;
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

           StartCoroutine(SoundManager.Fade(_drumsMusic, 0.2f, 0.15f));
        }
        else if (collision.CompareTag("Player") && _activateAMA)
        {
            GetComponent<Animator>().SetBool("openChest", true);
            _audioSource.Play();
            _collectorAnimator.SetBool("toggle", false);
            _amaAnimator.SetBool("toggle", true);
            _activateAMA = false;
            
            StartCoroutine(SoundManager.Fade(_stringsMusic, 0.2f, 0f));
            StartCoroutine(SoundManager.Fade(_drumsMusic, 1f, 0.03f));
            StartCoroutine(SoundManager.Fade(_bassMusic, 0.2f, 0.25f));
            StartCoroutine(SoundManager.Fade(_melodyMusic, 0.2f, 0.08f));
        }
        else if (collision.CompareTag("Player") && _deactivateAMA)
        {
            _amaAnimator.SetBool("toggle", false);
            _deactivateAMA = false;

            _playerMovement._groundSpeed = 8f;
            _playerMovement.airSpeed = 4f;
            
            StartCoroutine(SoundManager.Fade(_stringsMusic, 0.2f, 0.2f));
            StartCoroutine(SoundManager.Fade(_drumsMusic, 0.2f, 0.2f));
            StartCoroutine(SoundManager.Fade(_bassMusic, 0.2f, 0.2f));
            StartCoroutine(SoundManager.Fade(_melodyMusic, 0.2f, 0.2f));
        }
    }
}
