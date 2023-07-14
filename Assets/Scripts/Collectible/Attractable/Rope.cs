using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rope : ItemsToInventory, IAttractable
{
    public ItemData ropeData;
    
    public void CollectOnAttract()
    {
        GetInventoryValues(ropeData);
    }
}
