using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProximityPosition : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private Renderer _render;
    
    void Start()
    {
        _render = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        _render.sharedMaterial.SetVector("_PlayerPosition", _player.position);
    }
}
