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
    private bool _moveAtEnd;
    private bool _move;
    private int _listPosition;
    private int _endOfPath;

    #endregion
    
    private void Update()
    {
        if (_move)
        {
            MoveTo(_listPosition, _endOfPath);
            GetComponentInChildren<Animator>().SetBool("isMoving", true);
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
                transform.LookAt(_positionsToGo[i]);
            }
            else
            {
                //Intentar Look at player para tratar de evitar bugs visuales
                _move = false;
                GetComponentInChildren<Animator>().SetBool("isMoving", false);
                if (_moveAtEnd)
                {
                    GetComponent<FriendlyNPC>().StartRoaming();
                }
            }
        }
    }

    public void MoveToData(int listPosition, int endOfList, bool moveAtEnd)
    {
        this._listPosition = listPosition;
        this._endOfPath = endOfList;
        transform.LookAt(_positionsToGo[listPosition]);
        _move = true;
        this._moveAtEnd = moveAtEnd;
    }
}
