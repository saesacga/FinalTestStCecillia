using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class TutorialInventor : MonoBehaviour
{
    private Flowchart _currentFlowchart;
    private bool _collectorStep1Completed;
    private bool _collectorStep2Completed;
    private bool _collectorStep3Completed;
    private bool _collectorStep4Completed;
    private bool _collectorStep5Completed;
    
    void Start()
    {
        _currentFlowchart = GetComponentInChildren<Fungus.Flowchart>();
    }
    
    void Update()
    {
        if (ActionMapReference.playerMap.ChangeSchemes.EnableFarming.WasPerformedThisFrame() && _collectorStep1Completed == false)
        {
            _collectorStep1Completed = true;
            _currentFlowchart.ExecuteBlock("SuperrecolectoraTutorial1");
        }
        else if (Destructible._collectorStep2Completed && _collectorStep2Completed == false)
        {
            _collectorStep2Completed = true;
            _currentFlowchart.ExecuteBlock("SuperrecolectoraTutorial2");
        }
        else if (TheCollector._collectorStep3Completed && _collectorStep3Completed == false)
        {
            _collectorStep3Completed = true;
            _currentFlowchart.ExecuteBlock("SuperrecolectoraTutorial3");
        }
        else if (Constructible._collectorStep4Completed && _collectorStep4Completed == false)
        {
            _collectorStep4Completed = true;
            _currentFlowchart.ExecuteBlock("SuperrecolectoraTutorial4");
        }
        else if (TheCollector._collectorStep5Completed && _collectorStep5Completed == false)
        {
            _collectorStep5Completed = true;
            _currentFlowchart.ExecuteBlock("SuperrecolectoraTutorial5");
        }
    }
}
