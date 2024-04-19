using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySoundOneShot(AudioClip[] sounds, AudioSource source)
    {
        AudioClip clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        source.PlayOneShot(clip);
    }
    public static void PlaySoundOneShot(AudioClip[] sounds)
    {
        AudioClip clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        _audioSource.PlayOneShot(clip);
    }
}
