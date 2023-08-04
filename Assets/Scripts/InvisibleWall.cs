using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using Collision = UnityEngine.Collision;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private Flowchart _flowchart;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ActionMapReference.EnterInteraction(false);
            _flowchart.ExecuteBlock("InvisibleWallHit");
        }
    }
}
