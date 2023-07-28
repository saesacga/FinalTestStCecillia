using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FriendlyNPC : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _interactSprite;
    private Fungus.Flowchart _currentFlowchart;

    private bool interactuarFlag = false;
    private bool inDialog = false;

    private void Start()
    {
        _currentFlowchart = GetComponentInChildren<Fungus.Flowchart>();
    }

    void Update()
    {
        if (ActionMapReference.playerMap.Interaccion.Interactuar.WasPressedThisFrame() && interactuarFlag)
        {
            ActionMapReference.playerMap.Movimiento.Disable();
            ActionMapReference.playerMap.Combate.Disable();
            ActionMapReference.playerMap.PauseMap.Disable();
            _currentFlowchart.ExecuteBlock("DeafultBlock");
            
            inDialog = true;
        }

        if (interactuarFlag == true)
        {
            float fade = Mathf.Lerp(1f, 0f, 0.2f);
            _interactSprite.color = new Color(1, 1, 1, fade);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { interactuarFlag = true; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { interactuarFlag = false; }
            float fade = Mathf.Lerp(0f, 1f, 0.2f);
        _interactSprite.color = new Color(1, 1, 1, fade);
    }

    public void ExitBlock()
    {
        inDialog = false;  
            if(inDialog == false) 
            { 
                ActionMapReference.ActivateMaps();
            }
    }
}
