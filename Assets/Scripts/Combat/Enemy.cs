using System;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private enum State{Roaming,ChaseTarget}
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    public int health;
    CustomProjectile customProjectile;
    private AIDestinationSetter _aiDestination;
    
    private Vector3 _lastposition;
    private State _state;
    
    private void Start()
    {
        _aiDestination = GetComponent<AIDestinationSetter>();
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        _state = State.Roaming; 

        _lastposition = transform.position;
    }

    private void Update()
    {
        switch (_state)
        {
            default:
            case State.Roaming:
                if (transform.position == _lastposition) { roamPosition = GetRoamingPosition(); }
                _lastposition = transform.position;
        
                if (_aiDestination.target == null && _aiDestination.ai != null) { _aiDestination.ai.destination = roamPosition; }
                if (Vector3.Distance(transform.position,roamPosition) < 1f) { roamPosition = GetRoamingPosition(); }
                break;
            case State.ChaseTarget:
                break;
        }
        
        if (health <= 0) { Destroy(this.gameObject); }
    }
    private Vector3 GetRoamingPosition()
    {
        return startingPosition + GetRandomDir() * Random.Range(10f, 20f);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<AIPath>().maxSpeed = 15;
            _state = State.ChaseTarget;
            _aiDestination.target = collider.transform;   
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<AIPath>().maxSpeed = 5;
            _aiDestination.target = null;
            _state = State.Roaming;
        }
    }

    public void EnemyDamage(int damage)
    {
        health -= damage;
    }
    
    //generate random normalized direction
    public static Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
}

