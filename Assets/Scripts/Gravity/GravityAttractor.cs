using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [SerializeField] private float _gravity = -10f;
    [HideInInspector] public Vector3 gravityUp; 
    
    public void GravityAttraction(Transform body)
    {
        gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;
        
        body.GetComponent<Rigidbody>().AddForce(gravityUp * _gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
