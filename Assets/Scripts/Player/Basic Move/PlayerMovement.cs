using System;
using System.Collections;
using System.Collections.Generic;
using SpriteGlow;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Members

    #region For Movement

    public CharacterController controller;
    private Vector3 move;

    private float speed = 12f;
    [SerializeField] private float _groundSpeed = 12f;
    [SerializeField] private float airSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    #endregion
    
    #region For Dash
    
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    private bool _inDash;
    private float _dashContador;
    [SerializeField] private GameObject _amaGameObject;
    [SerializeField] private Image _dashAvailableImage;
    [SerializeField] private Animator _dashAnimator;
    [SerializeField] private AudioClip[] _antiGravitySounds;
    private AudioSource _audioSource;
    
    #endregion

    #region For Jump

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool _isGrounded;

    #endregion

    #region For Smothness
    
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myInput;
    [SerializeField] private float smoothInputSpeed;
    
    #endregion

    #region For WallJump

    private bool wallJumpFlag;
    private bool _allowWallJumpInput;
    private GameObject _currentWall;
    [SerializeField] private AudioClip[] _wallJumpsSounds;
    
    #endregion

    private TutorialUI _tutorialUI;
    private bool _tutorialWJUI;
    private bool _tutorialTPUI;
    private bool _tutorialAntiGravity;
    [SerializeField] private Image _tpTutoImage;
    
    #endregion

    private void Start()
    {
        _tutorialUI = GetComponent<TutorialUI>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        #region Wall Jump

        if (_isGrounded) { _currentWall = null; }
        
        if (wallJumpFlag && _isGrounded == false) 
        {
            if (velocity.y < -2.5f || _inDash)
            {
                if (_inDash)
                {
                    StopCoroutine(_dashRoutine);
                    StartCoroutine(SoundManager.Fade(_audioSource, 0.5f, 0f));
                    speed = airSpeed;
                    _dashAnimator.SetBool("toggleAG", false);
                    _inDash = false;
                    if (_crossable != null) { _crossable.isTrigger = false; }
                }
                velocity.y = 0f;
                SoundManager.PlaySoundOneShot(_wallJumpsSounds);
                _allowWallJumpInput = true;
                _wallJumpTimerRoutine = null;
                _wallJumpTimerRoutine = WallJumpTimer(1.5f);
                StartCoroutine(_wallJumpTimerRoutine);
            }
        }
        if (ActionMapReference.playerInput.actions["Jumping"].WasPerformedThisFrame() && _allowWallJumpInput)
        {
            WallJump();                
        }
        
        #endregion

        #region Jump
        
        else if (ActionMapReference.playerInput.actions["Jumping"].WasPerformedThisFrame() && _isGrounded && _inDash == false)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            speed = airSpeed;
        }
        
        #endregion
        
        #region Dash
        
        if (_dashContador <= _dashCooldown) 
        { 
            _dashContador += Time.deltaTime; 
            _dashAvailableImage.color = Color.red; 
        } 
        else
        {
            if (_amaGameObject.activeInHierarchy) { _dashAvailableImage.color = Color.yellow; }
        }
        if (ActionMapReference.playerInput.actions["Dash"].WasPerformedThisFrame() && _dashContador >= _dashCooldown && _amaGameObject.activeInHierarchy && _isGrounded == false)
        {
            _dashRoutine = null;
            _dashRoutine = Dash();
            StartCoroutine(_dashRoutine);
        }
        
        #endregion

        #region GroundCheck
        
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (_isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            speed = _groundSpeed;
        }
        else if(_inDash == false)
        {
            speed = airSpeed;
        }

        if (velocity.y < -15) { velocity.y = -15f; }
        
        #endregion
        
        #region Movimiento
        
        myInput.x = ActionMapReference.playerInput.actions["Move"].ReadValue<Vector2>().x;
        myInput.z = ActionMapReference.playerInput.actions["Move"].ReadValue<Vector2>().y;

        currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
        move = transform.right * currentInputVector.x + transform.forward * currentInputVector.z;

        if (_allowWallJumpInput == false)
        {
            controller.Move(move * (speed * Time.deltaTime)); //Movimiento horizontal (Caminar)
            controller.Move(velocity * Time.deltaTime); //Movimiento vertical (Salto)
        }

        if (_allowWallJumpInput == false && _inDash == false) { velocity.y += gravity * Time.deltaTime; }
        
        #endregion

        if (_tutorialWJUI && TeleportProjectile.tpTuto == false)
        {
            _tutorialUI.ShowControlsUI(0);
        }
        else if (_tutorialTPUI && TeleportProjectile.tpTuto == false)
        {
            _tutorialUI.ShowControlsUI(1);
            _tpTutoImage.color = _tutorialUI.imageTutorial.color;
        }
        else if (_tutorialAntiGravity && TeleportProjectile.tpTuto == false)
        {
            _tutorialUI.ShowControlsUI(2);
        }
        else
        {
            _tpTutoImage.color = Color.clear;
            _tutorialUI.HideControlsUI();
        }
    }

    private static Collider _crossable;
    private IEnumerator _dashRoutine;
    private IEnumerator Dash()
    {
        _inDash = true;
        speed = _groundSpeed;
        float startTime = Time.time;
        _dashAnimator.SetBool("toggleAG", true);
         
        SoundManager.PlayOnLoop(_antiGravitySounds, _audioSource);
        StartCoroutine(SoundManager.Fade(_audioSource, 0.2f, 0.8f));
         
        while (Time.time <= startTime + _dashTime && _allowWallJumpInput == false)
        {
            velocity.y = 0;
            _dashContador = 0;

            #region Raycast
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.transform.CompareTag("Crossable"))
                {
                    _crossable = hit.collider;
                    _crossable.isTrigger = true;
                }
            }
            #endregion
            
            yield return null;
        }
        StartCoroutine(SoundManager.Fade(_audioSource, 0.5f, 0f));
        _dashAnimator.SetBool("toggleAG", false);
        _inDash = false;
        speed = airSpeed;
        if (_crossable != null) { _crossable.isTrigger = false; }
    }

    private void WallJump()
    {
        wallJumpFlag = false;
        _allowWallJumpInput = false;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //Hacer un salto
        _dashContador = _dashCooldown;
        StopCoroutine(_wallJumpTimerRoutine);
    }

    private IEnumerator _wallJumpTimerRoutine;
    private IEnumerator WallJumpTimer(float time)
    {
        yield return new WaitForSeconds(time);
        wallJumpFlag = false;
        _allowWallJumpInput = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("WallJump") && _amaGameObject.activeInHierarchy)
        {
            if (_currentWall == collision.collider.gameObject) { return; }
            _currentWall = collision.collider.gameObject;
            
            wallJumpFlag = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("WallJump") && _amaGameObject.activeInHierarchy)
        {
            wallJumpFlag = false;
            _allowWallJumpInput = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("WallJump"))
        {
            _tutorialWJUI = true;
        }
        else if (collider.CompareTag("TeleportSurface"))
        {
            _tutorialTPUI = true;
        }
        else if (collider.CompareTag("AntiGravityTuto"))
        {
            _tutorialAntiGravity = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        _tutorialTPUI = false;
        _tutorialWJUI = false;
        _tutorialAntiGravity = false;
    }
}
