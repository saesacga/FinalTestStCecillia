using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    [SerializeField] private Transform _orientation;
    
    private float xRotation = 0f;

    //For smoothness
    private Vector3 currentInputMouseVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myMouseInput;
    [SerializeField] private float smoothInputSpeedForCamera;
    [SerializeField] private Camera _cinematicCamera;
    public static bool _cutsceneInProgress = true;

    void Update()
    {
        if (_cutsceneInProgress) { _cinematicCamera.enabled = true; GetComponent<Camera>().enabled = false; }
        else { _cinematicCamera.enabled = false; GetComponent<Camera>().enabled = true; }
        
        myMouseInput.x = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        myMouseInput.y = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        currentInputMouseVector = Vector3.SmoothDamp(currentInputMouseVector, myMouseInput, ref smoothInputVelocity, smoothInputSpeedForCamera);

        xRotation -= currentInputMouseVector.y; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); 
        _orientation.Rotate(Vector3.up * currentInputMouseVector.x);
    }
}
