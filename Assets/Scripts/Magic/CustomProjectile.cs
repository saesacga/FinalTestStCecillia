using UnityEngine;

public class CustomProjectile : MonoBehaviour
{
    #region Declarations

    public Rigidbody rigidBody;
    public float maxLifeTime;
    [SerializeField] private GameObject _particle;

    PhysicMaterial newPhysicsMat;

    #endregion
    
    private void Update()
    {
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0) { Destroy(); }
    }
    
    private void Destroy()
    {
        _particle = Instantiate(_particle, transform.position, Quaternion.identity);
        float totalDuration = _particle.GetComponent<ParticleSystem>().main.duration + _particle.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
        Destroy(_particle, totalDuration);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Destroy();
        }
    }

    /*private void Setup()
    {
        //Create a new physics material
        newPhysicsMat = new PhysicMaterial();
        newPhysicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
        newPhysicsMat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = newPhysicsMat;

        //Set gravity
        rigidBody.useGravity = false;
    }*/
}
