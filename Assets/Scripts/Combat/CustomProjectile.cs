using UnityEngine;

public class CustomProjectile : MonoBehaviour
{
    #region Declarations

    public Rigidbody rigidBody;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;

    //Lifetime
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;

    //Sticky
    public bool itsSticky;

    int collisions;
    PhysicMaterial newPhysicsMat;

    #endregion
    
    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //When to explode
        if (collisions > maxCollisions) { Explode(); }

        //Count down lifetime
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0) { Explode(); }
    }

    private void Explode()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies); 
        for (int i = 0; i < enemies.Length; i++) 
        { 
            enemies[i].GetComponent<Enemy>().EnemyDamage(explosionDamage);
        }
     
        //Delay for evasion of bugs
        Invoke("Delay", 0.05f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Count up collisions
        collisions++;

        //Explode if hits an enemy and explodeOnTouch is activated
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
            collision.collider.GetComponent<Enemy>().EnemyDamage(explosionDamage);
        }

        if (!collision.collider.CompareTag("Player") && itsSticky == true)
        {
            rigidBody.useGravity = false;
            rigidBody.drag = 10;
            rigidBody.constraints = RigidbodyConstraints.FreezePosition;
            //rigidBody.isKinematic = true;
            if (collision.collider.CompareTag("Enemy")) 
            {
                Transform transformSticky = collision.collider.GetComponent<Transform>();
                transform.SetParent(transformSticky);
            }
        }
    }

    private void Setup()
    {
        //Create a new physics material
        newPhysicsMat = new PhysicMaterial();
        newPhysicsMat.bounciness = bounciness;
        newPhysicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
        newPhysicsMat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = newPhysicsMat;

        //Set gravity
        rigidBody.useGravity = useGravity;
    }
}
