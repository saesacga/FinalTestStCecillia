using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using Collision = UnityEngine.Collision;

public class LightRay : MonoBehaviour
{
    [SerializeField] private Collider _invisibleWall;

    public int _starsRequired;
    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;
    private bool _alreadyExecuted;

    private void Update()
    {
        if (_starsRequired == _starsInPosition && _alreadyExecuted == false)
        {
            GetComponent<Animator>().SetBool("activateRay", true);
            
            GetComponent<MoveToPositions>()._move = true;
            
            _invisibleWall.isTrigger = true;
            _alreadyExecuted = true;
        }
    }
}
