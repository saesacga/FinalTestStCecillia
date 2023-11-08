using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportProjectile : MonoBehaviour
{
    [HideInInspector] public bool allowedToTeleport = false; 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("TeleportSurface"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().drag = 10;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            allowedToTeleport = true;
        }
        else
        {
            Debug.Log("ItWasDestroy");
            TeleportArtifact._canThrow = true;
            Destroy(this.gameObject);
        }
    }
}
