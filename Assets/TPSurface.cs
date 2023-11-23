using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSurface : MonoBehaviour
{
    private void AllowTPAnimator(int allow)
    {
        if (TeleportProjectile.animatorCanChangeValues)
        {
            if (allow == 0) { TeleportProjectile.allowedToTeleport = true; }
            
            else { TeleportProjectile.allowedToTeleport = false; }
        }
    }
}
