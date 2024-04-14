using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//PUEDE SER ÚTIL PARA DESPUÉS: InputSystem.onAnyButtonPress.CallOnce(ctrl => Debug.Log($"{ctrl} pressed")); NECESITA: using UnityEngine.InputSystem.Utilities;
public class ActionMapReference : MonoBehaviour
{
    #region Singleton
    
    public static PlayerInput playerInput;
    public static bool isGamepad;
    
    #endregion

    [SerializeField] private List<GameObject> _gameplaySchemes = new List<GameObject>();
    [SerializeField] private Animator _cinemachineBlendRef;
    private static Animator _cinemachineBlend;
    public static bool camTransitioning;

    [SerializeField] private bool _lockCursor;
    
    void OnEnable()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += OnControlsChanged;
    }
 
    void OnDisable() {
        playerInput.onControlsChanged -= OnControlsChanged;
    }
 
    void OnControlsChanged(PlayerInput input) {
        isGamepad = input.currentControlScheme.Equals("Gamepad");
    }
    
    private void Start()
    {
        _cinemachineBlend = _cinemachineBlendRef;
        Cursor.visible = false;
        playerInput.actions.FindAction("Move").Disable();
        ActivateAllMaps(); //BORRAR ESTO DESPUÉS DE TESTEO
        if (_lockCursor) { Cursor.lockState = CursorLockMode.Locked; }
    }

    private void Update()
    {
        ChangeGameplaySchemes();
    }

    public static void ActivateAllMaps() //NOTA: NUNCA ACTIVAR TODOS LOS MAPAS: playerMap.Enable();
    {
        Cursor.visible = false;
        
        playerInput.actions.FindAction("Look").Enable();
        playerInput.actions.FindAction("Jumping").Enable();
        
        //playerInput.actions.FindAction("Move").Enable(); //Eliminar esta linea 
        
        playerInput.actions.FindActionMap("Combate").Enable();
        playerInput.actions.FindActionMap("Farming").Enable();
        playerInput.actions.FindActionMap("MovimientoAvanzado").Enable();
        playerInput.actions.FindActionMap("PauseMap").Enable();
        playerInput.actions.FindActionMap("ChangeSchemes").Enable(); //Para testeo
        
        playerInput.actions.FindActionMap("UI").Disable();
    }
    
    public static void ActivateUINavigation()
    {
        Cursor.visible = true;
        playerInput.actions.FindActionMap("Combate").Disable();
        playerInput.actions.FindActionMap("Farming").Disable();
        playerInput.actions.FindAction("Look").Disable();
        playerInput.actions.FindAction("Jumping").Disable();
        playerInput.actions.FindActionMap("MovimientoAvanzado").Disable();
        playerInput.actions.FindActionMap("ChangeSchemes").Disable();
        playerInput.actions.FindActionMap("PauseMap").Enable();
        playerInput.actions.FindActionMap("UI").Enable();
    }

    private void ChangeGameplaySchemes()
    {
        if (playerInput.actions["EnableCombat"].WasPerformedThisFrame())
        {
            _gameplaySchemes[0].SetActive(true);
            _gameplaySchemes[1].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerInput.actions["EnableFarming"].WasPerformedThisFrame())
        {
            _gameplaySchemes[1].SetActive(true);
            _gameplaySchemes[0].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerInput.actions["EnableAdvanceMovement"].WasPerformedThisFrame())
        {
            _gameplaySchemes[2].SetActive(true);
            _gameplaySchemes[0].SetActive(false);
            _gameplaySchemes[1].SetActive(false);
        }
    }
    
    public static IEnumerator ActivateLooking(bool state, string camBlendName)
    {
        if (state) 
        {
            _cinemachineBlend.Play(camBlendName);
            yield return new WaitForSeconds(1.6f);
            camTransitioning = false;
            playerInput.actions.FindActionMap("Movimiento").Enable();
            playerInput.actions.FindAction("Move").Enable();
        }
        else
        {
            camTransitioning = true;
            playerInput.actions.FindActionMap("Movimiento").Disable();
            playerInput.actions.FindAction("Move").Disable();
            yield return new WaitForSeconds(0.1f);
            _cinemachineBlend.Play(camBlendName);
        }
    }
}
