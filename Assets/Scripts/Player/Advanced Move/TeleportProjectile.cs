using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class TeleportProjectile : MonoBehaviour
{ 
    public static bool allowedToTeleport;
    public static bool animatorCanChangeValues;
    private static GameObject _tpSurface;
    
    private TutorialUI _tutorialUI;
    public static bool tpTuto;
    
    private void Start()
    {
        _tutorialUI = GetComponent<TutorialUI>();
    }

    private void OnDestroy()
    {
        tpTuto = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("TeleportSurface") || collision.collider.CompareTag("TeleportSurfaceTuto")) //Comparar con otro tag especifico del tp del tutorial
        {
            if (collision.collider.CompareTag("TeleportSurfaceTuto")) { tpTuto = true; }
            
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            transform.SetParent(collision.collider.GetComponent<Transform>());

            if (collision.collider.GetComponent<TPSurface>() == null) { allowedToTeleport = true; }
            else
            {
                animatorCanChangeValues = true; 
                if (_tpSurface == null)
                {
                    _tpSurface = collision.collider.gameObject;
                    _tpSurface.GetComponent<TPSurface>().canChangeValues = true;
                }
                else if (collision.collider.gameObject != _tpSurface)
                {
                    _tpSurface.GetComponent<TPSurface>().canChangeValues = false; 
                    _tpSurface = collision.collider.gameObject; 
                    _tpSurface.GetComponent<TPSurface>().canChangeValues = true;
                }
            }
        }
        else
        {
            allowedToTeleport = false;
            animatorCanChangeValues = false;
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (tpTuto)
        {
            _tutorialUI.ShowControlsUI(0);
        }
        else
        {
            _tutorialUI.HideControlsUI();
        }
        
        if (ActionMapReference.playerInput.actions["DestruirTeleport"].WasPerformedThisFrame())
        {
            allowedToTeleport = false;
            animatorCanChangeValues = false;
            Destroy(this.gameObject);
        }
    }
}
