using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    
    private float xRotation = 0f;

    //For smoothness
    private Vector3 currentInputMouseVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myMouseInput;
    [SerializeField] private float smoothInputSpeedForCamera;
    [HideInInspector] public float mouseSensitivityAimAssist;

    #region Necesario para planeta

    //[SerializeField] private Transform _orientation; PARA PLANETA
    //[SerializeField] private Camera _cinematicCamera;
    //public static bool _cutsceneInProgress = true;

    #endregion
    
    private void Start()
    {
        mouseSensitivityAimAssist = mouseSensitivity;
    }

    void Update()
    {
        myMouseInput.x = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().x * mouseSensitivityAimAssist * Time.deltaTime;
        myMouseInput.y = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().y * mouseSensitivityAimAssist * Time.deltaTime;

        currentInputMouseVector = Vector3.SmoothDamp(currentInputMouseVector, myMouseInput, ref smoothInputVelocity, smoothInputSpeedForCamera);

        xRotation -= currentInputMouseVector.y; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
        playerBody.Rotate(Vector3.up * currentInputMouseVector.x);
        
        #region Necesario para planeta
        
        //if (_cutsceneInProgress) { _cinematicCamera.enabled = true; GetComponent<Camera>().enabled = false; } PARA PLANETA
        //else { _cinematicCamera.enabled = false; GetComponent<Camera>().enabled = true; } 
        //_orientation.Rotate(Vector3.up * currentInputMouseVector.x);
        
        #endregion
    }
}
