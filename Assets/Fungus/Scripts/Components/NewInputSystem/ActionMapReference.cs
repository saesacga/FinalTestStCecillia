using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

//PUEDE SER ÚTIL PARA DESPUÉS: InputSystem.onAnyButtonPress.CallOnce(ctrl => Debug.Log($"{ctrl} pressed")); NECESITA: using UnityEngine.InputSystem.Utilities;
public class ActionMapReference : MonoBehaviour
{
    #region Singleton
    
    static public PlayerMap playerMap;
    [SerializeField] private CinemachineInputProvider _cinemachineInputRef;
    private static CinemachineInputProvider _cinemachineInputProvider;
    
    #endregion

    [SerializeField] private List<GameObject> _gameplaySchemes = new List<GameObject>();
    [SerializeField] private Animator _cinemachineBlendRef;
    private static Animator _cinemachineBlend;

    private void Start()
    {
        _cinemachineInputProvider = _cinemachineInputRef;
        _cinemachineBlend = _cinemachineBlendRef;
        Cursor.visible = false;
        playerMap = new PlayerMap();
        ActivateAllMaps(); //BORRAR ESTO DESPUÉS DE TESTEO
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ChangeGameplaySchemes();
    }

    public static void ActivateAllMaps() //NOTA: NUNCA ACTIVAR TODOS LOS MAPAS: playerMap.Enable();
    {
        Cursor.visible = false;
        playerMap.Combate.Enable();
        playerMap.Movimiento.Enable();
        playerMap.ChangeSchemes.EnableCombat.Enable();
        playerMap.Interaccion.Enable();
        playerMap.PauseMap.Enable();
        
        //PARA TESTEO
        
        playerMap.ChangeSchemes.EnableFarming.Enable(); 
        playerMap.Farming.Enable();
        playerMap.ChangeSchemes.EnableAdvanceMovement.Enable(); 
        playerMap.MovimientoAvanzado.Enable();
        
        //FINAL DEL TESTEO
        
        playerMap.UI.Disable();
    }
    
    public static void EnterInteraction(bool inCutscene) //NOTA: NUNCA ACTIVAR TODOS LOS MAPAS: playerMap.Enable();
    {
        playerMap.Combate.Disable();
        playerMap.Farming.Disable();
        playerMap.Movimiento.Disable();
        playerMap.MovimientoAvanzado.Disable();
        playerMap.ChangeSchemes.Disable();
        playerMap.PauseMap.Disable();
        playerMap.UI.Disable();
        if (inCutscene == false) { playerMap.Movimiento.Look.Enable(); }
        playerMap.Interaccion.Enable();
    }

    public static void ActivateUINavigation()
    {
        Cursor.visible = true;
        playerMap.Combate.Disable();
        playerMap.Farming.Disable();
        playerMap.Movimiento.Disable();
        playerMap.MovimientoAvanzado.Disable();
        playerMap.ChangeSchemes.Disable();
        playerMap.Interaccion.Disable();
        playerMap.UI.Enable();
    }

    private void ChangeGameplaySchemes()
    {
        if (playerMap.ChangeSchemes.EnableCombat.WasPerformedThisFrame())
        {
            _gameplaySchemes[0].SetActive(true);
            _gameplaySchemes[1].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerMap.ChangeSchemes.EnableFarming.WasPerformedThisFrame())
        {
            _gameplaySchemes[1].SetActive(true);
            _gameplaySchemes[0].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerMap.ChangeSchemes.EnableAdvanceMovement.WasPerformedThisFrame())
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
            yield return new WaitForSeconds(0.3f);
            _cinemachineInputProvider.enabled = true;
        }
        else
        {
            _cinemachineInputProvider.enabled = false;
            yield return new WaitForSeconds(0.1f);
            _cinemachineBlend.Play(camBlendName);
        }
    }
}
