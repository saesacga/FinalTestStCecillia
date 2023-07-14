using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private Vector3 move;

    public float speed = 12f;
    public float airSpeedReduction = 7f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    private bool inTheAir = false;

    //For Smothness
    private Vector3 currentInputVector;
    private Vector3 smoothInputVelocity;
    private Vector3 myInput;
    [SerializeField] private float smoothInputSpeed;

    private bool wallJumpFlag;
    private bool wallJump = true;

    private float _contador = 0;
    void Update()
    {
        //Jump
        if (wallJumpFlag == true && wallJump == true && inTheAir == true && ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            wallJump = false;
        }

        if (ActionMapReference.playerMap.Movimiento.Jumping.WasPerformedThisFrame() == true && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            speed -= airSpeedReduction;
            inTheAir = true;
        }

        #region Dash
        if (_contador <= _dashCooldown) { _contador += Time.deltaTime; }
        if (ActionMapReference.playerMap.MovimientoAvanzado.Dash.WasPerformedThisFrame() && _contador >= _dashCooldown)
        {
            StartCoroutine(Dash());
        }
        #endregion
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (inTheAir == true)
            {
                speed += airSpeedReduction;
                inTheAir = false;
            }
        }

        #region Movimiento
        myInput.x = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().x;
        myInput.z = ActionMapReference.playerMap.Movimiento.Move.ReadValue<Vector2>().y;

        currentInputVector = Vector3.SmoothDamp(currentInputVector, myInput, ref smoothInputVelocity, smoothInputSpeed);
        move = transform.right * currentInputVector.x + transform.forward * currentInputVector.z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        #endregion
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        
        while (Time.time < startTime + _dashTime && move != new Vector3(0f,0f,0f))
        {
            controller.Move(move * _dashSpeed * Time.deltaTime);
            _contador = 0;

            #region Raycast
            Ray ray = GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("WallJump")) { wallJumpFlag = true; }
        else { wallJumpFlag = false; }
    }
    private void OnCollisionExit(Collision collision)
    {
        wallJump = true;
    }
}
