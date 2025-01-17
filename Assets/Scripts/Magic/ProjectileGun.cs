using System;
using SpriteGlow;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ProjectileGun : MonoBehaviour
{
    #region Members
    
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject _magicParticles;

    [SerializeField] private float shootForce;
    [SerializeField] private float timeBetweenShooting;
    private bool shooting, readyToShoot;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask _rayCastLayerMaskIgnore;
    
    [SerializeField] private PlayersLook _playersLook;
    
    [SerializeField] private float _sensitivityNoAssist = 150;
    [SerializeField] private float _aimAssistValue = 60;

    public bool allowInvoke = true;
    
    [SerializeField] private AudioClip[] _shootSounds;
    private AudioSource _audioSource;
    
    #endregion
    
    private void OnDisable()
    {
        _playersLook.mouseSensitivity = _sensitivityNoAssist;
    }

    private TutorialUI _tutorialUI;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _tutorialUI = GetComponent<TutorialUI>();
        readyToShoot = true;
    }

    private bool _aimAssist = true;
    private bool _shootCompleted;
    
    private void Update()
    {
        if (readyToShoot && ActionMapReference.playerInput.actions["Fire"].IsPressed())
        {
            GetComponentInChildren<Animator>().Play("ShootAnimation", -1, 0f);
            Shoot();
        }

        if (_shootCompleted == false)
        {
            _tutorialUI.ShowControlsUI(0);
        }
        else
        {
            _tutorialUI.HideControlsUI();
        }
        
        ShootRayCheck();
    }
    
    #region For Shooting and Aim Assist

    private Vector3 targetPoint;
    private void ShootRayCheck()
    {
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, _rayCastLayerMaskIgnore))
        {
            targetPoint = hit.point;
            if (hit.collider.CompareTag("Star") || hit.collider.CompareTag("Enemy"))
            {
                if (_aimAssist && ActionMapReference.isGamepad) { _playersLook.mouseSensitivity =  _aimAssistValue; }
            }
            else
            {
                _playersLook.mouseSensitivity = _sensitivityNoAssist;
            }
        }
        else
        {
            targetPoint = ray.GetPoint(75);
            _playersLook.mouseSensitivity = _sensitivityNoAssist;
        }
    }

    private GameObject _magicParticlesRef;
    private void Shoot()
    {
        readyToShoot = false;
        
        SoundManager.PlaySoundOneShot(_shootSounds, _audioSource);
        
        Vector3 direction = targetPoint - attackPoint.position;
        
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
        
        if (_magicParticles != null)
        {
            if (_magicParticlesRef != null) { Destroy(_magicParticlesRef); }
            _magicParticlesRef = Instantiate(_magicParticles, attackPoint.position, Quaternion.identity);
            _magicParticlesRef.transform.SetParent(transform);
        }
        
        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        _shootCompleted = true;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
    
    #endregion
    
    #region For Configuration Menu

    public void ChooseSensitivity(float sensitivity)
    {
        _sensitivityNoAssist = sensitivity;
        _playersLook.mouseSensitivity = _sensitivityNoAssist;
    }
    public void ToggleAimAssist(bool toggle)
    {
        _aimAssist = toggle;
    }

    #endregion
}
