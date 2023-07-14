using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FriendlyNPC : MonoBehaviour
{   
    [SerializeField]private bool interactuarFlag = false;

    public SpriteRenderer interactSprite;
    public Fungus.Flowchart myFlowchart;

    public bool inDialog = false;
    
    void Update()
    {
        if (ActionMapReference.playerMap.Interaccion.Interactuar.WasPressedThisFrame() && interactuarFlag)
        {
            ActionMapReference.playerMap.Movimiento.Disable();
            ActionMapReference.playerMap.Combate.Disable();
            ActionMapReference.playerMap.PauseMap.Disable();
            myFlowchart.ExecuteBlock("Dialog");
            inDialog = true;
        }

        if (interactuarFlag == true)
        {
            float fade = Mathf.Lerp(1f, 0f, 0.2f);
            interactSprite.color = new Color(1, 1, 1, fade);
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
        interactSprite.color = new Color(1, 1, 1, fade);
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
