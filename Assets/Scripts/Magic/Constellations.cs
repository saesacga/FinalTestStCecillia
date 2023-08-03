using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using Collision = UnityEngine.Collision;

public class Constellations : MonoBehaviour
{
    [SerializeField] private Collider _invisibleWall;
    [SerializeField] private Flowchart _flowchart;

    public int _starsRequired;
    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;
    private bool _alreadyExecuted;

    private void Update()
    {
        if (_starsRequired == _starsInPosition && _alreadyExecuted == false)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Animator>().SetBool("startDancing", true);
            _invisibleWall.isTrigger = true;
            ActionMapReference.EnterInteraction(false);
            _flowchart.StopAllBlocks();
            _flowchart.ExecuteBlock("ConstelacionCompleta");
            _alreadyExecuted = true;
        }
    }
}
