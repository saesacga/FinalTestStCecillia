using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//PUEDE SER ÚTIL PARA DESPUÉS: InputSystem.onAnyButtonPress.CallOnce(ctrl => Debug.Log($"{ctrl} pressed")); NECESITA: using UnityEngine.InputSystem.Utilities;
public class ActionMapReference : MonoBehaviour
{
    #region Singleton
    
    static public PlayerMap playerMap;
    
    #endregion

    [SerializeField] private List<GameObject> _gameplaySchemes = new List<GameObject>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        playerMap = new PlayerMap();
        //ActivateAllMaps();
    }

    private void Update()
    {
        ChangeGameplaySchemes();
    }

    public static void ActivateAllMaps() //NOTA: NUNCA ACTIVAR TODOS LOS MAPAS: playerMap.Enable();
    {
        Cursor.visible = false;
        playerMap.Combate.Enable();
        playerMap.Farming.Enable();
        playerMap.Movimiento.Enable();
        playerMap.MovimientoAvanzado.Enable();
        playerMap.Interaccion.Enable();
        playerMap.PauseMap.Enable();
        playerMap.UI.Disable();
    }
    
    public static void EnterInteraction() //NOTA: NUNCA ACTIVAR TODOS LOS MAPAS: playerMap.Enable();
    {
        playerMap.Combate.Disable();
        playerMap.Farming.Disable();
        playerMap.Movimiento.Disable();
        playerMap.MovimientoAvanzado.Disable();
        playerMap.Interaccion.Enable();
        playerMap.PauseMap.Disable();
        playerMap.UI.Disable();
    }

    public static void ActivateUINavigation()
    {
        Cursor.visible = true;
        playerMap.Combate.Disable();
        playerMap.Farming.Disable();
        playerMap.Movimiento.Disable();
        playerMap.MovimientoAvanzado.Disable();
        playerMap.Interaccion.Disable();
        playerMap.UI.Enable();
    }

    private void ChangeGameplaySchemes()
    {
        if (playerMap.Movimiento.EnableCombat.WasPerformedThisFrame())
        {
            _gameplaySchemes[0].SetActive(true);
            _gameplaySchemes[1].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerMap.Movimiento.EnableFarming.WasPerformedThisFrame())
        {
            _gameplaySchemes[1].SetActive(true);
            _gameplaySchemes[0].SetActive(false);
            _gameplaySchemes[2].SetActive(false);
        }
        else if (playerMap.Movimiento.EnableAdvanceMovement.WasPerformedThisFrame())
        {
            _gameplaySchemes[2].SetActive(true);
            _gameplaySchemes[0].SetActive(false);
            _gameplaySchemes[1].SetActive(false);
        }
    }
}
