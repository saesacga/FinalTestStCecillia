using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FriendlyNPC : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _interactSprite;
    private Fungus.Flowchart _currentFlowchart;

    private bool interactuarFlag = false;

    private void Start()
    {
        _currentFlowchart = GetComponentInChildren<Fungus.Flowchart>();
    }

    void Update()
    {
        if (interactuarFlag == true)
        {
            float fade = Mathf.Lerp(1f, 0f, 0.2f);
            _interactSprite.color = new Color(1, 1, 1, fade);
            
            if (ActionMapReference.playerMap.Interaccion.Interactuar.WasPressedThisFrame())
            {
                ActionMapReference.EnterInteraction();
                _interactSprite.enabled = false;
                _currentFlowchart.ExecuteBlock("DeafultBlock");
            }
        }
        else
        {
            float fade = Mathf.Lerp(0f, 1f, 0.2f); 
            _interactSprite.color = new Color(1, 1, 1, fade);
        }
    }

    public void OnInteraction()
    {
        PlayersLook._cutsceneInProgress = true;
        ActionMapReference.EnterInteraction();
    }
    public void ExitBlock()
    {
        PlayersLook._cutsceneInProgress = false;
        _interactSprite.enabled = true;
        ActionMapReference.ActivateAllMaps();
    }
    
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) interactuarFlag = true;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) interactuarFlag = false; 
    }
}
