using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersMovementRB : MonoBehaviour
{
    #region Members
    
    #region For Movement
    
    [SerializeField] private float _speed;
    [SerializeField] private float _slopeForce;
    [SerializeField] private Transform _orientation;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GravityAttractor _gravityAttractor;
    
    #endregion
    
    #region For Jump
    
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded;
    private bool _jumpPerformed;
    private float _groundDistance = 0.4f;
    private bool _exitSlope;

    #endregion
    
    #region For Smothness
    
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myInput;
    [SerializeField] private float smoothInputSpeed;
    
    #endregion
    
    #endregion

    private void FixedUpdate()
    {
        Move();

        if (_jumpPerformed) { Jump(); }
    }

    void Update()
    {
        #region Input para movimiento
        
        myInput.x = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x;
        myInput.z = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y;

        currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
        
        #endregion

        if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame())
        {
            _jumpPerformed = true;
        }
    }
    
    private void Jump()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        Vector3 jumpForces = Vector3.zero;

        if (_isGrounded)
        {
            jumpForces = Vector3.up * _jumpForce;
            _rigidbody.AddRelativeForce(jumpForces, ForceMode.VelocityChange);
        }
        
        _jumpPerformed = false;
        _exitSlope = true;
    }
    
    private void Move()
    {
        #region Gravedad

        Vector3 gravity = (transform.position - _gravityAttractor.transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(gravity * _gravityAttractor.gravity, ForceMode.Acceleration);

        Vector3 bodyUp = transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravity) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.deltaTime);

        #endregion
        
        #region Movimiento
        
        #region Usando velocity
        
        float verticalSpeed = Vector3.Dot(transform.up, _rigidbody.velocity);
        _rigidbody.velocity = (_orientation.right * (currentInputVector.x * _speed)) + (transform.up * verticalSpeed) + (_orientation.forward * (currentInputVector.z * _speed));
        //_rigidbody.velocity = (transform.right * (currentInputVector.x * _speed)) + (transform.up * verticalSpeed) + (transform.forward * (currentInputVector.z * _speed));
        
        #endregion

        #region Intento con MovePosition (Problemas con las colisiones)

        //Vector3 moveDir = new Vector3(currentInputVector.x, 0f, currentInputVector.z);
        //_rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(moveDir) * _speed * Time.deltaTime);
        
        #endregion
        
        #region Intento con addforce (falló)
        /*
        //Find target velocity
        Vector3 currentVelocity = _rigidbody.velocity;
        Vector3 targetVelocity = new Vector3(currentInputVector.x, 0f, currentInputVector.z);
        targetVelocity *= _speed;
       
        //Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);
       
        //Calculate forces
        Vector3 _velocityChange = (targetVelocity - currentVelocity); 
        _velocityChange = new Vector3(_velocityChange.x , (gravity.y * _gravityAttractor.gravity), _velocityChange.z);
       
        //Limit force
        Vector3.ClampMagnitude(_velocityChange, _maxForce);
       
        _rigidbody.AddForce(_velocityChange, ForceMode.VelocityChange);
        */
        #endregion
        
        #endregion
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Slope") && currentInputVector != new Vector3(0f,0f,0f) && _exitSlope == false) { _rigidbody.AddForce(new Vector3(0f, -_slopeForce, 0f)); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Slope")) { _exitSlope = false; }
    }
}
