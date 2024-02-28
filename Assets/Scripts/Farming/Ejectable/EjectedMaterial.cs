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

    [SerializeField] private GameObject _particle;

    private void OnEnable()
    {
        StartCoroutine(DestroyOnSeconds(6));
        
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, _ejectVelocity * Time.deltaTime);
    }

    private void Destroy()
    {
        _particle = Instantiate(_particle, transform.position, Quaternion.identity);
        float totalDuration = _particle.GetComponent<ParticleSystem>().main.duration + _particle.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
        Destroy(_particle, totalDuration);
        Destroy(this.gameObject);
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        IConstruible construible = collision.gameObject.GetComponent<IConstruible>();
        if (construible != null)
        {
            construible.Consturct(itemData);
            Destroy();
        }
    }
    
    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Palomas"))
        {
            Palomas palomas = collision.collider.GetComponent<Palomas>();
            palomas.StartEvent(itemData);
            Destroy();
            return;
        }
        
        if (!collision.collider.CompareTag("Collector") && !collision.collider.CompareTag("Player"))
        {
            Destroy();
        }
    }
    private IEnumerator DestroyOnSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy();
    }
}
