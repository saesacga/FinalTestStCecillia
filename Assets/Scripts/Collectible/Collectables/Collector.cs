using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{ 
    private void OnTriggerStay(Collider collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();
        if (collectible != null)
        {
            collectible.Collect();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();
        if (collectible != null)
        {
            collectible.CannotCollect();
        }
    }
}
