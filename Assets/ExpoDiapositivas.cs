using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpoDiapositivas : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _slides;
    [SerializeField] private Animator _cinemachineBlend;
    private int _slideNumber;
    
    void Start()
    {
        _cinemachineBlend.Play("MatriarcaCam");
        ActionMapReference.playerInput.actions.FindActionMap("Interaccion").Enable();
        ActionMapReference.playerInput.actions.FindAction("Look").Disable();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        _spriteRenderer.sprite = _slides[_slideNumber];

        if (ActionMapReference.playerInput.actions["NextSlide"].WasPressedThisFrame() && _slideNumber < (_slides.Length - 1))
        {
            _slideNumber++;
        }
        else if (ActionMapReference.playerInput.actions["PreviousSlide"].WasPressedThisFrame() && _slideNumber > 0)
        {
            _slideNumber--;
        }

        if (ActionMapReference.playerInput.actions["ExitDiapos"].WasPressedThisFrame())
        {
            _cinemachineBlend.Play("POVCam");
            ActionMapReference.playerInput.actions.FindAction("Look").Enable();
            ActionMapReference.playerInput.actions.FindAction("Move").Enable();
            ActionMapReference.playerInput.actions.FindActionMap("Interaccion").Disable();
        }
    }
}
