using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpThrowTrajectory : MonoBehaviour
{
    
    #region Declarations

    [Header("Line renderer variables")] 
    [SerializeField] private LineRenderer _line;
    [Range(2,30)]
    [SerializeField] private int _resolution;
    
    [Header("Formula variables")]
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private float _yLimit;
    private float _gravity;
    
    [Header("Linecast variables")]
    [Range(2,30)]
    [SerializeField] private int _lineCastResolution;
    [SerializeField] private LayerMask _canHit;
    
    #endregion

    private void Start()
    {
        _gravity = Mathf.Abs(Physics.gravity.y);
    }

    private void Update()
    {
        StartCoroutine(RenderArc());      
        if (ActionMapReference.playerMap.Movimiento.Trayectoria.IsPressed()) { }
    }

    private IEnumerator RenderArc()
    {
        _line.positionCount = _resolution + 1;
        _line.SetPositions(CalculateLineArray());
        yield return null;
    }

    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[_resolution + 1];
        
        var lowestTimeValueX = MaxTimeX() / _resolution;
        var lowestTimeValueZ = MaxTimeZ() / _resolution;
        var lowestTimeValue = lowestTimeValueX > lowestTimeValueZ ? lowestTimeValueZ : lowestTimeValueX;

        for (int i = 0; i < lineArray.Length; i++)
        {
            var t = lowestTimeValue * i;
            lineArray[i] = CalculateLinePoint(t);
        }

        return lineArray;
    }

    private Vector3 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / _lineCastResolution;

        for (int i = 0; i < _lineCastResolution + 1; i++)
        {
            RaycastHit rayHit;
            
            var t = lowestTimeValue * i; //current frame
            var tt = lowestTimeValue * (i + 1); //next frame

            if (Physics.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), out rayHit, _canHit))
            {
                return rayHit.point;
            }
        }

        return CalculateLinePoint(MaxTimeY());
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = _velocity.x * t;
        float z = _velocity.z * t;
        float y = (_velocity.y * t) - (_gravity * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
    }

    private float MaxTimeY()
    {
        var v = _velocity.y;
        var vv = v * v;

        var t = (v + Mathf.Sqrt(vv + 2 * _gravity * (transform.position.y - _yLimit))) / _gravity;
        return t;
    }
    private float MaxTimeX()
    {
        if (ValueIsAlmostZero(_velocity.x)) { SetValueToAlmostZero(ref _velocity.x); }
        
        var x = _velocity.x;
        
        var t = (HitPosition().x - transform.position.x) / x;
        return t;
    }
    private float MaxTimeZ()
    {
        if (ValueIsAlmostZero(_velocity.z)) { SetValueToAlmostZero(ref _velocity.z); }
        
        var z = _velocity.z;
        
        var t = (HitPosition().z - transform.position.z) / z;
        return t;
    }

    private bool ValueIsAlmostZero(float value)
    {
        return value < 0.0001f && value > 0.0001f;
    }

    private void SetValueToAlmostZero(ref float value)
    {
        value = 0.0001f;
    }
}
