using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPositions : MonoBehaviour
{
    #region Members
    
    [SerializeField] private List<Vector3> _positionsToGo = new List<Vector3>();
    [SerializeField] private AnimationCurve _velocityCurve;
    private float _velocity;
    private float _velocityOverTime;
    [HideInInspector] public bool _move;
    private int _listPosition;
    private int _endOfPath;

    #endregion
    
    private void Update()
    {
        if (_move)
        {
            MoveTo(_listPosition, _endOfPath);
        }
    }

    private void MoveTo(int i, int end)
    {
        #region Velocity Curve

        if (_velocityOverTime <= 15f)
        {
            _velocity = _velocityCurve.Evaluate(_velocityOverTime);
            _velocityOverTime+=0.01f;
        }
            
        #endregion
        
        transform.position = Vector3.MoveTowards(transform.position, _positionsToGo[i], _velocity * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, _positionsToGo[i]) < 2f)
        {
            if (_listPosition != end)
            {
                _listPosition++;
            }
            else
            {
                _move = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(_listPosition >= 1)
        { 
            _listPosition++;
            _endOfPath++;
            _move = true;
        }
    }
}
