using System;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ProjectileGun : MonoBehaviour
{
    #region Members
    
    public GameObject bullet;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;
    [SerializeField] private LayerMask _rayCastLayerMaskIgnore;
    
    public TextMeshProUGUI ammunitionDisplay;
    [SerializeField] private GameObject _canvas;

    public bool allowInvoke = true;
    
    #endregion

    private void OnEnable()
    {
        _canvas.SetActive(true);
    }

    private void OnDisable()
    {
        _canvas.SetActive(false);
    }

    private void Start()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();
        
        //Set ammo display
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }
    private void MyInput()
    {
        //auto or semi auto
        if (allowButtonHold)
        {
            shooting = ActionMapReference.playerMap.Combate.Fire.IsPressed();
        }
        else
        {
            shooting = ActionMapReference.playerMap.Combate.Fire.WasPressedThisFrame();
        }
        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
        //reloading
        if (ActionMapReference.playerMap.Combate.Recargar.WasPressedThisFrame() && bulletsLeft < magazineSize && !reloading) 
        {
            Reload();
        }
        //reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //find hit position
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        //check if raycast hits something
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _rayCastLayerMaskIgnore))
        {
            targetPoint = hit.point;
        }
        //if not then pick a point far away from the player
        else { targetPoint = ray.GetPoint(75); }

        //calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //instantiate projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithoutSpread.normalized;
        //add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        //for bouncing projectiles
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        
        //Instiantiate muzzle flash
        /*if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }*/

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if more than one bulletsPerTap repeat shoot function (shotguns for example)
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        { 
            Invoke("Shoot", timeBetweenShooting); 
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
