using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EjectedMaterial : MonoBehaviour, IEjectable
{
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public Vector3 targetPoint;
    [Range(10f,40f)]
    [SerializeField] private float _ejectVelocity;

    private void OnEnable()
    {
        StartCoroutine(DestroyOnSeconds(6));
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, _ejectVelocity * Time.deltaTime);
    }
    public void OnTriggerEnter(Collider collider)
    {
        IConstruible construible = collider.GetComponent<IConstruible>();
        if (construible != null)
        {
            construible.Consturct(itemData);
            Destroy(this.gameObject);
        }
    }
    public void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Collector") && !collision.collider.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
        
    }
    private IEnumerator DestroyOnSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
