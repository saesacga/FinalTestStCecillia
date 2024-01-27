using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

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
        myMouseInput.y = ActionMapReference.playerMap.Movimiento.Look.ReadValue<Vector2>().y * mouseSensitivityAimAssist * Time.deltaTime;

        currentInputMouseVector = Vector3.SmoothDamp(currentInputMouseVector, myMouseInput, ref smoothInputVelocity, smoothInputSpeedForCamera);

        xRotation -= currentInputMouseVector.y; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //Camara arriba y abajo
    }
}