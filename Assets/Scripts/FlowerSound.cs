using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    private void OnEnable()
    {
        //Flower.OnFlowerCollected += FlowerOnOnFlowerCollected;
    }
    private void OnDisable()
    {
        //Flower.OnFlowerCollected -= FlowerOnOnFlowerCollected;
    }
    
    private void FlowerOnOnFlowerCollected()
    {
        _audioSource.Play();
    }
}
