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
            _flowchart.ExecuteBlock("InvisibleWallHit");
        }
    }

    public void DisableInvisibleWall()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
