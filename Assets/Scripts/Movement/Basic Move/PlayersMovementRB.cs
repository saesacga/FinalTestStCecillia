using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersMovementRB : MonoBehaviour
{
    #region Members
    
    [SerializeField] private float _speed, _maxForce;
    [SerializeField] private Rigidbody _rigidbody;
    
    #region For Smothness
    
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myInput;
    [SerializeField] private float smoothInputSpeed;
    
    #endregion
    
    #endregion

    private void FixedUpdate()
    {
        #region Movimiento

        //Find target velocity
        Vector3 currentVelocity = _rigidbody.velocity;
        Vector3 targetVelocity = new Vector3(currentInputVector.x, 0, currentInputVector.z);
        targetVelocity *= _speed;
        
        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);
        
        //Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        
        //Limit force
        Vector3.ClampMagnitude(velocityChange, _maxForce);
        
        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        #endregion
    }

    void Update()
    {
        #region Input para movimiento
        
        myInput.x = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x;
        myInput.z = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y;

        currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
        
        #endregion
    }
}
