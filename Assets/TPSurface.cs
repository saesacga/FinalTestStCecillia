using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSurface : MonoBehaviour
{
    public bool canChangeValues;
    private void AllowTPAnimator(int allow)
    {
        if (canChangeValues && TeleportProjectile.animatorCanChangeValues)
        {
            if (allow == 0) { TeleportProjectile.allowedToTeleport = true; }
            
            else { TeleportProjectile.allowedToTeleport = false; }
        }
    }
}
