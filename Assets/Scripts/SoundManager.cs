using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.VsCodeDebugger.SDK;
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

    public static void PlayOnLoop(AudioClip[] sounds, AudioSource source)
    {
        source.Stop();
        AudioClip clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    public static IEnumerator Fade(AudioSource source, float duration, float targetVolume)
    {
        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        
        yield break;
    }
}
