using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGate : MonoBehaviour
{ 
    [SerializeField] private Animator _cinemachineBlend;
    [SerializeField] private Transform _playerPos;
    [SerializeField] private Vector3 _newPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GetComponent<Animator>().SetBool("EnterForest", true);
        }
    }

    private void SetCamera(string cameraBlend)
    {
        if (cameraBlend == "ForestGate1Cam")
        {
            StartCoroutine(ActionMapReference.ActivateLooking(false, cameraBlend));
        }
        else if (cameraBlend == "POVCam")
        {
            StartCoroutine(ActionMapReference.ActivateLooking(true, cameraBlend));
        }
        
    }

    private void TeleportToTheForest()
    {
        _playerPos.position = _newPosition;
    }
}
