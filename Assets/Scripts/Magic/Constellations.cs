using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class Constellations : MonoBehaviour
{
    public int _starsRequired;
    [SerializeField] private Flowchart _flowchart;

    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;

    private void Update()
    {
        if (_starsRequired == _starsInPosition)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<Animator>().SetBool("startDancing", true);
            _flowchart.ExecuteBlock("DeafultBlock");
        }
    }
}
