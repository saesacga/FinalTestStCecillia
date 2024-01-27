using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    
    private float yRotation = 0f;

    //For smoothness
    private Vector3 currentInputMouseVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myMouseInput;
    [SerializeField] private float smoothInputSpeedForCamera;
    [HideInInspector] public float mouseSensitivityAimAssist;
    
    private void Start()
    {
        mouseSensitivityAimAssist = mouseSensitivity;
    }

    void Update()
    {
        myMouseInput.x = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().x * mouseSensitivityAimAssist * Time.deltaTime;

        currentInputMouseVector = Vector3.SmoothDamp(currentInputMouseVector, myMouseInput, ref smoothInputVelocity, smoothInputSpeedForCamera);
        
        yRotation += currentInputMouseVector.x;

        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f); //Camara izquierda a derecha
    }
}
