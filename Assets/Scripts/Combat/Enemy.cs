using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public Vector3 scaleReduction;
    public Vector3 deafultScale;

    CustomProjectile customProjectile;

    //private Respawn respawn;

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void EnemyDamage(int damage)
    {
        health -= damage;
    }
}

