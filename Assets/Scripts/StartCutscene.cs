using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class StartCutscene : MonoBehaviour
{
    private Constructible _constructible;
    private bool cutsceneInProgress;
    [SerializeField] private Flowchart _flowchart;

    private void Start()
    {
        _constructible = GetComponent<Constructible>();
    }

    private void Update()
    {
        if (_constructible.constructed && cutsceneInProgress == false)
        {
            ActionMapReference.EnterInteraction(false);
            _flowchart.ExecuteBlock("DiscursoCutscene");
            cutsceneInProgress = true;
        }
    }

    public void TeleportObject(GameObject objectTP, Vector3 position)
    {
        objectTP.transform.position = position;
    }
}
