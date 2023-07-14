using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBullet : MonoBehaviour
{
    public ProjectileGun projectileGun;
    public CustomProjectile customProjectile;
    public Enemy enemy;
    public void ItsSticky(bool stickyValue)
    {
        customProjectile.itsSticky = stickyValue;
    }
    public void ChangeForce(float force)
    {
        projectileGun.shootForce = force;
    }
    public void TimeBetweenShooting(float time)
    {
        projectileGun.timeBetweenShooting = time;
    }
    public void ReloadTime(float rtime)
    {
        projectileGun.reloadTime = rtime;
    }
    public void AllowButtonHold(bool allow)
    {
        projectileGun.allowButtonHold = allow;
    }
    public void Bounce(float bounce)
    {
        customProjectile.bounciness = bounce;
    }
    public void UseGravity (bool allowGravity)
    {
        customProjectile.useGravity = allowGravity;
    }
    public void ExplosionDamage(float damage)
    {
        customProjectile.explosionDamage = (int)damage;
    }
    public void ExplosionRange (float range)
    {
        customProjectile.explosionRange = range;
    }
    public void MaxLifeTime (float maxLife)
    {
        customProjectile.maxLifeTime = maxLife;
    }
    public void ExplodeOnTouch(bool onTouch)
    {
        customProjectile.explodeOnTouch = onTouch;
    }
    public void EnemyLife(float enemyLife)
    {
        enemy.health = (int)enemyLife;
    }
}
