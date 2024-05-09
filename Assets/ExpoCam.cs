using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpoCam : MonoBehaviour
{ 
    [SerializeField] private Animator _cinemachineBlend;
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            ActionMapReference.camTransitioning = true;
            ActionMapReference.playerInput.actions.FindAction("Look").Disable();
            _cinemachineBlend.Play("PalomasCam");
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _cinemachineBlend.Play("POVCam");
            ActionMapReference.camTransitioning = false;
            ActionMapReference.playerInput.actions.FindAction("Look").Enable();
        }
    }
}
