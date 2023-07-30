using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersMovementRB : MonoBehaviour
{
    #region Members
    
    #region For Movement
    
    [SerializeField] private float _speed;
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
    
    private float _slopeForce;
    private float _surfaceAngle;
    private enum SlopeState{normal,front,right,left,back}
    private SlopeState _slopeState;
    private bool _onSlope;

    [SerializeField] private float _slopeExtraGravity;
    [SerializeField] private float _rayLenght;
    [SerializeField] private LayerMask _slopeRayMask;
    [SerializeField] private Transform _slopeRayPosition;

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

    private void Start()
    {
        _slopeState = SlopeState.normal;
    }

    private void FixedUpdate()
    {
        Move();

        if (_jumpPerformed) { Jump(); }
    }

    void Update()
    {
        GetAngle();
        
        #region Movimiento state machine
        
        switch (_slopeState)
        {
            default:
            case SlopeState.normal:
                myInput.x = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x; 
                myInput.z = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y;
                currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
                break;
            case SlopeState.front:
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -0.3f, 0.3f); 
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y, -1f, 0f);
                currentInputVector = myInput;
                break;
            case SlopeState.back:
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -0.3f, 0.3f); 
                myInput.z = Mathf.Clamp01(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y);
                currentInputVector = myInput;
                break;
            case SlopeState.right:
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -1f, 0f);
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y,-0.3f, 0.3f);
                currentInputVector = myInput;
                break;
            case SlopeState.left:
                myInput.x = Mathf.Clamp01(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x);
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y,-0.3f, 0.3f);
                currentInputVector = myInput;
                break;
        }
        
        #endregion

        #region Wall Jump Related

        _inTheAir = !Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (_inTheAir == false) { _currentWall = null; }

        if (_allowWallJump)
        {
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
    }
    
    private void Move()
    {
        #region Gravedad

        Vector3 gravityUp = (transform.position - _gravityAttractor.transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(gravityUp * (_gravityAttractor.gravity * _slopeForce), ForceMode.Acceleration);

        Vector3 bodyUp = transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;
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
        
        _jumpPerformed = false;
        //_exitingSlope = true;
        
        Vector3 jumpForces = Vector3.zero;
        
        if (_isGrounded || _allowWallJump)
        {
            jumpForces = Vector3.up * _jumpForce;
            _rigidbody.AddRelativeForce(jumpForces, ForceMode.VelocityChange);
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
        }
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
        RaycastHit hit;

        if (Physics.SphereCast(_slopeRayPosition.position, 0.5f, _slopeRayPosition.forward, out hit, _rayLenght, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            
            if (_surfaceAngle > 50f) { _slopeState = SlopeState.front; }
            else { _slopeState = SlopeState.normal; }
        }
        else if (Physics.SphereCast(_slopeRayPosition.position, 0.5f, -_slopeRayPosition.forward, out hit,  _rayLenght, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            
            if (_surfaceAngle > 50f) { _slopeState = SlopeState.back; }
            else { _slopeState = SlopeState.normal; }
        }
        else if (Physics.SphereCast(_slopeRayPosition.position, 0.5f, _slopeRayPosition.right, out hit, _rayLenght, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            
            if (_surfaceAngle > 50f) { _slopeState = SlopeState.right; }
            else { _slopeState = SlopeState.normal; }
        }
        else if (Physics.SphereCast(_slopeRayPosition.position, 0.5f, -_slopeRayPosition.right, out hit, _rayLenght, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            
            if (_surfaceAngle > 50f) { _slopeState = SlopeState.left; }
            else { _slopeState = SlopeState.normal; }
        }
        else if (Physics.Raycast(_slopeRayPosition.position, -_slopeRayPosition.up, out hit, 10, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            _slopeState = SlopeState.normal;
        }
        else { _surfaceAngle = 0; _slopeState = SlopeState.normal; }
        
        if (_surfaceAngle is < 50f and > 1f && currentInputVector != new Vector3(0f, 0f, 0f)) { _slopeForce = _slopeExtraGravity; }
        else { _slopeForce = 1; }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        #region Wall Jump
        
        if (collision.collider.CompareTag("WallJump"))
        {
            if (_currentWall == collision.collider.gameObject) { return; }
            _currentWall = collision.collider.gameObject;
            
            if (_inTheAir)
            {
                _wallJumpContador = 0;
                _allowWallJump = true;
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;  
            }
        }

        #endregion
    }
    private void OnCollisionExit(Collision collision)
    {
        #region Wall Jump
        
        if (collision.collider.CompareTag("WallJump"))
        {   
            _allowWallJump = false;
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
        }

        #endregion
    }
}
