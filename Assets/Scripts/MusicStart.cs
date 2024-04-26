using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStart : MonoBehaviour
{
    [SerializeField] private AudioSource _stringsMusic;
    [SerializeField] private AudioSource _drumsMusic;
    [SerializeField] private AudioSource _bassMusic;
    [SerializeField] private AudioSource _melodyMusic;
    
    private void MusicOnStart(int state)
    {
        if (state == 0) //Juego inicia
        {
            GetComponent<AudioSource>().Play();
        }
        else
        {
            _stringsMusic.Play();
            _drumsMusic.Play();
            _bassMusic.Play();
            _melodyMusic.Play();
        }
    }
}
