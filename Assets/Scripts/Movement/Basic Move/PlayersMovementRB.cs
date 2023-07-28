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
    
    #region For Dash

    [SerializeField] private AnimationCurve _dashSpeedMultiplierCurve;
    private float _dashSpeed = 1;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    private float _dashContador = 99;

    #endregion
    
    #region For Jump
    
    [SerializeField] private float _jumpForce;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    private bool _isGrounded;
    private bool _jumpPerformed;
    private float _groundDistance = 0.4f;
    
    #endregion

    #region For Slopes

    private bool _exitSlope;
    private float _surfaceAngle;
    [SerializeField] private Transform _frontRayPosition;
    [SerializeField] private PhysicMaterial _playerPhysicMaterial;

    #endregion

    #region For WallJump

    private bool _allowWallJump;
    private bool _inTheAir;
    private float _wallJumpContador;
    private GameObject _currentWall;

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

        #region Wall Jump Related

        _inTheAir = !Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        switch (_wallJumpContador)
        {
            case <= 1.5f:
                _wallJumpContador += Time.deltaTime;
                break;
            case >= 1.5f:
                _allowWallJump = false;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
                break;
        }

        #endregion

        #region Dash Related

        if (_dashContador <= _dashCooldown) { _dashContador += Time.deltaTime; }
        if (ActionMapReference.playerMap.MovimientoAvanzado.Dash.WasPerformedThisFrame() && _dashContador >= _dashCooldown)
        {
            StartCoroutine(Dash());
        }

        #endregion

        if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame())
        {
            _jumpPerformed = true;
        }
        
        GetAngle();
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
        _rigidbody.velocity = (_orientation.right * (currentInputVector.x * _speed * _dashSpeed * (Time.deltaTime * 100))) + (transform.up * verticalSpeed) + (_orientation.forward * (currentInputVector.z * _speed * _dashSpeed * (Time.deltaTime * 100)));
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
    
    private void Jump()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        Vector3 jumpForces = Vector3.zero;

        if (_isGrounded || _allowWallJump)
        {
            jumpForces = Vector3.up * _jumpForce;
            _rigidbody.AddRelativeForce(jumpForces, ForceMode.VelocityChange);
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
        }
        
        _jumpPerformed = false;
        _exitSlope = true;
    }
    
    private IEnumerator Dash()
    {
        float _dashSpeedOverTime = 0f;
        float startTime = Time.time;
        
        while (Time.time <= startTime + _dashTime && currentInputVector != new Vector3(0f,0f,0f))
        {
            _dashSpeed = _dashSpeedMultiplierCurve.Evaluate(_dashSpeedOverTime);
            _dashSpeedOverTime+=0.01f;
            _dashContador = 0;

            #region Raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.transform.CompareTag("Crossable"))
                {
                    hit.collider.isTrigger = true; 
                    yield return new WaitForSeconds(_dashTime); 
                    hit.collider.isTrigger = false;
                }
            }
            #endregion
            
            yield return null;
        }
        _dashSpeed = 1;
    }

    private void GetAngle()
    {
        _frontRayPosition.rotation = Quaternion.Euler(-gameObject.transform.rotation.x, 0f, 0f);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal); //Parece funcionar, seguir haciendo pruebas
            _surfaceAngle = Vector3.Angle(localNormal, Vector3.up);
            Debug.Log(_surfaceAngle);
        }
        else
        {
            _surfaceAngle = 0f;
            Debug.Log(_surfaceAngle);   
        }
        /*RaycastHit frontHit;
        if (Physics.Raycast(_frontRayPosition.position, _frontRayPosition.TransformDirection(-Vector3.forward), out frontHit,Mathf.Infinity))
        {
            Debug.DrawRay(_frontRayPosition.position, _frontRayPosition.TransformDirection(-Vector3.forward) * frontHit.distance, Color.yellow);
            _surfaceAngle = Vector3.Angle(frontHit.normal, Vector3.up);
            Debug.Log(_surfaceAngle);
        }*/
    }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Slope") && currentInputVector != new Vector3(0f,0f,0f) && _exitSlope == false) { _rigidbody.AddForce(new Vector3(0f, -_slopeForce, 0f)); }
        if(collision.collider.CompareTag("Slope")){ GetComponent<Collider>().material = null; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        #region Slopes

        if (collision.collider.CompareTag("Slope")) { _exitSlope = false; }
        
        #endregion

        #region Wall Jump

        if (_inTheAir == false) { _currentWall = null; }
        
        if (_currentWall == collision.collider.gameObject) { return; }
        
        _currentWall = collision.collider.gameObject;
        
        if (collision.collider.CompareTag("WallJump") && _inTheAir)
        {
            _wallJumpContador = 0;
            _allowWallJump = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        #endregion
    }
    private void OnCollisionExit(Collision collision)
    {
        _allowWallJump = false;
        if (collision.collider.CompareTag("WallJump"))
        {
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
        }
        
        GetComponent<Collider>().material = _playerPhysicMaterial;
    }
}
