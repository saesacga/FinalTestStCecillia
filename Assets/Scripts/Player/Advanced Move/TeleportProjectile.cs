using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportProjectile : MonoBehaviour
{ 
    public static bool allowedToTeleport;
    public static bool animatorCanChangeValues;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("TeleportSurface"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            transform.SetParent(collision.collider.GetComponent<Transform>());

            if (collision.collider.GetComponent<Animator>() == null) { allowedToTeleport = true; }
            else { animatorCanChangeValues = true; }
        }
        else
        {
            Debug.Log("ItWasDestroy");
            allowedToTeleport = false;
            animatorCanChangeValues = false;
            TeleportArtifact._canThrow = true;
            Destroy(this.gameObject);
        }
    }
}
