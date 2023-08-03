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
    private float _groundDistance = 0.5f;
    private float _jumpCooldown = 0.5f;
    
    #endregion

    #region For Slopes
    
    [SerializeField] private float _slownessOnCollision;
    [SerializeField] private float _slopeExtraGravity;
    [SerializeField] private LayerMask _slopeRayMask;
    [SerializeField] private PhysicMaterial _playerMaterial;
    [SerializeField] private Transform _slopeRayPosition;
    private float _rayLenght = 0.7f;
    private float _surfaceAngle;
    private enum SlopeState{normal,front,right,left,back}
    private SlopeState _slopeState;
    private bool _exitSlope;

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
    private float smoothInputSpeed = 0.1f;
    
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
        
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
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
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -_slownessOnCollision, _slownessOnCollision); 
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y, -1f, 0f);
                currentInputVector = myInput;
                break;
            case SlopeState.back:
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -_slownessOnCollision, _slownessOnCollision); 
                myInput.z = Mathf.Clamp01(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y);
                currentInputVector = myInput;
                break;
            case SlopeState.right:
                myInput.x = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x, -1f, 0f);
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y,-_slownessOnCollision, _slownessOnCollision);
                currentInputVector = myInput;
                break;
            case SlopeState.left:
                myInput.x = Mathf.Clamp01(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x);
                myInput.z = Mathf.Clamp(ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y,-_slownessOnCollision, _slownessOnCollision);
                currentInputVector = myInput;
                break;
        }
        
        //Debug.Log(_slopeState);

        #endregion

        #region Wall Jump Related
        
        if (_isGrounded) { _currentWall = null; }

        if (_allowWallJump)
        {
            switch (_wallJumpContador)
            {
                case < 1.5f:
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

        #region Jump Related
        
        switch (_jumpCooldown)
        { 
            case < 0.5f: 
                _jumpCooldown += Time.deltaTime; 
                break;
            case >= 0.5f:
                if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame()) { _jumpPerformed = true; }
                _exitSlope = false;
                break;
        }
        
        #endregion
        
    }
    
    private void Move()
    {
        #region Gravedad

        Vector3 gravityUp = (transform.position - _gravityAttractor.transform.position).normalized;
        if (_surfaceAngle is < 50f and > 1f && _exitSlope == false && _jumpPerformed == false) //On slope gravity
        {
            GetComponent<Rigidbody>().AddForce(gravityUp * (_gravityAttractor.gravity * _slopeExtraGravity), ForceMode.Acceleration);
            
            if (myInput == new Vector3()) { GetComponent<Collider>().material = null; }
            else { GetComponent<Collider>().material = _playerMaterial; }
        }
        else //Normal gravity
        {
            GetComponent<Rigidbody>().AddForce(gravityUp * _gravityAttractor.gravity, ForceMode.Acceleration);
            GetComponent<Collider>().material = _playerMaterial;
        }
        
        Vector3 bodyUp = transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.deltaTime);

        #endregion
        
        #region Movimiento
        
        float verticalSpeed = Vector3.Dot(transform.up, _rigidbody.velocity);
        _rigidbody.velocity = (_orientation.right * (currentInputVector.x * _speed * _dashSpeed * (Time.deltaTime * 100))) + (transform.up * verticalSpeed) + (_orientation.forward * (currentInputVector.z * _speed * _dashSpeed * (Time.deltaTime * 100)));
        
        #endregion
    }
    
    private void Jump()
    {
        Vector3 jumpForces = Vector3.zero;
        
        if (_isGrounded || _allowWallJump)
        {
            _jumpCooldown = 0f;
            _exitSlope = true;
            jumpForces = Vector3.up * _jumpForce;
            _rigidbody.AddRelativeForce(jumpForces, ForceMode.VelocityChange);
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezePosition;
        }
        _jumpPerformed = false;
    }
    
    private IEnumerator Dash()
    {
        float _dashSpeedOverTime = 0f;
        float startTime = Time.time;
        
        while (Time.time <= startTime + _dashTime && myInput != new Vector3(0f,0f,0f))
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
        else if (Physics.Raycast(_slopeRayPosition.position, -_slopeRayPosition.up, out hit, 1f, _slopeRayMask))
        {
            Vector3 localNormal = hit.transform.InverseTransformDirection(hit.normal);
            if (hit.collider.CompareTag("WallJump")) { _surfaceAngle = 0f; }
            else { _surfaceAngle = Vector3.Angle(localNormal, Vector3.up); }
            _slopeState = SlopeState.normal;
        }
        else { _surfaceAngle = 0; _slopeState = SlopeState.normal; }

        //if (_surfaceAngle is < 50f and > 1f && _exitSlope == false) { _slopeForce = _slopeExtraGravity; GetComponent<Collider>().material = null; }
        //else { _slopeForce = 1; GetComponent<Collider>().material = _playerMaterial; }
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
