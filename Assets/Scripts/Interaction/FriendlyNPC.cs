using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class FriendlyNPC : MonoBehaviour
{
    #region Members

    #region For Interaction

    [SerializeField] private SpriteRenderer _interactSprite;
    private Fungus.Flowchart _currentFlowchart;
    private bool interactuarFlag = false;
    
    #endregion

    #region For Movement

    public bool autoMove;
    private AIDestinationSetter _aiDestination;
    private Vector3 _startingPosition;
    private Vector3 _roamPosition;
    private float _counterToNextPos = 20f;
    private enum State { roaming, stand }
    private State _moveState;

    #endregion

    #endregion
    

    private void Start()
    {
        _currentFlowchart = GetComponentInChildren<Fungus.Flowchart>();

        #region For Movement

        _startingPosition = transform.position;
        _aiDestination = GetComponent<AIDestinationSetter>();
        if (autoMove)
        {
            _moveState = State.roaming;
        }
        else
        {
            _moveState = State.stand;
        }

        #endregion
    }

    void Update()
    {
        #region For Movement

        switch (_moveState)
        {
            case State.roaming:
                if (_counterToNextPos < 20f) { _counterToNextPos += Time.deltaTime; }
                else 
                { 
                    _aiDestination.ai.destination = GetRoamingPosition();
                    _counterToNextPos = Random.Range(0f, 15f); 
                }
                break;
            case State.stand:
                if (autoMove) { _aiDestination.ai.destination = transform.position; }
                break;       
        }

        #region For Move Animation

        if (GetComponentInChildren<Animator>() != null && autoMove) 
        { 
            GetComponentInChildren<Animator>().SetBool("isMoving", !GetComponent<AIPath>().reachedEndOfPath);
        }

        #endregion
        
        #endregion

        #region Interaction

        if (interactuarFlag == true)
        {
            float fade = Mathf.Lerp(1f, 0f, 0.2f);
            _interactSprite.color = new Color(1, 1, 1, fade);
            
            if (ActionMapReference.playerMap.Interaccion.Interactuar.WasPressedThisFrame())
            {
                ActionMapReference.EnterInteraction(false);
                _interactSprite.enabled = false;
                _currentFlowchart.ExecuteBlock("DeafultBlock");
            }
        }
        else
        {
            float fade = Mathf.Lerp(0f, 1f, 0.2f); 
            _interactSprite.color = new Color(1, 1, 1, fade);
        }

        #endregion
    }
    
    public void OnCutscene()
    {
        PlayersLook._cutsceneInProgress = true;
        ActionMapReference.EnterInteraction(true);
    }
    public void ExitBlock()
    {
        PlayersLook._cutsceneInProgress = false;
        _interactSprite.enabled = true;
        ActionMapReference.ActivateAllMaps();
    }

    public void StartRoaming()
    {
        _startingPosition = transform.position;
        autoMove = true;
        _moveState = State.roaming;
    }
    
    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Enemy.GetRandomDir() * Random.Range(5f, 20f);
    }
    
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactuarFlag = true;
            if (autoMove) { _moveState = State.stand; }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) interactuarFlag = false; 
        if(autoMove) _moveState = State.roaming;
    }
}
