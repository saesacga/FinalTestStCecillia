using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;

public class TutorialYerbatera : MonoBehaviour
{
    private Flowchart _currentFlowchart;
    private bool _amaStep1Completed;
    private bool _amaStep2Completed;
    private bool _amaStep3Completed;
    private bool _amaStep4Completed;
    private bool _amaStep5Completed;
    
    void Start()
    {
        _currentFlowchart = GetComponentInChildren<Fungus.Flowchart>();
    }
    
    void Update()
    {
        if (ActionMapReference.playerMap.ChangeSchemes.EnableAdvanceMovement.WasPerformedThisFrame() && _amaStep1Completed == false)
        {
            _amaStep1Completed = true;
            _currentFlowchart.StopAllBlocks();
            _currentFlowchart.ExecuteBlock("AMATutorial1");
        }
        else if (PlayersMovementRB.amaStep2Completed && _amaStep2Completed == false)
        {
            _amaStep2Completed = true;
            _currentFlowchart.StopAllBlocks();
            _currentFlowchart.ExecuteBlock("AMATutorial2");
        }
        else if (PlayersMovementRB.amaStep3Completed && _amaStep3Completed == false)
        {
            _amaStep3Completed = true;
            _currentFlowchart.StopAllBlocks();
            _currentFlowchart.ExecuteBlock("AMATutorial3");
        }
        else if (TeleportArtifact.amaStep4Completed && _amaStep4Completed == false)
        {
            _amaStep4Completed = true;
            _currentFlowchart.StopAllBlocks();
            _currentFlowchart.ExecuteBlock("AMATutorial4");
        }
        else if (TeleportArtifact.amaStep5Completed && _amaStep5Completed == false)
        {
            _amaStep5Completed = true;
            _currentFlowchart.StopAllBlocks();
            _currentFlowchart.ExecuteBlock("AMATutorial5");
        }
    }
}
