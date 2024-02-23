using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    [SerializeField] private Transform _playerBody;
    private float xRotation = 0f;
    
    #region Smoothness

    private Vector3 currentInputMouseVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myMouseInput;
    [SerializeField] private float smoothInputSpeedForCamera;
    [HideInInspector] public float mouseSensitivityAimAssist;

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
        
        if (ActionMapReference.camTransitioning == false)
        {
            xRotation -= currentInputMouseVector.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //Referencia para la cámara arriba y abajo
            _playerBody.Rotate(Vector3.up * currentInputMouseVector.x); //Mover jugador de izquierda a derecha
        }
        else //Igualar la rotación del POV con los movimientos de cámara
        {
            transform.localRotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, 0f, 0f);
            xRotation = UnityEditor.TransformUtils.GetInspectorRotation(transform).x;
            _playerBody.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
    }
}